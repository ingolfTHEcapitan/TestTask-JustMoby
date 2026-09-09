using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.Score;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowModel: IDisposable
    {
        public event Action OnStatsChanged;
        
        private readonly PlayerStatsModel _statsModel;
        private readonly PlayerStatsSaveLoad _saveLoad;
        private readonly PlayerDeath _playerDeath;
        private readonly IGamePauseService _pauseService;
        private readonly IScoreService _scoreService;
        private readonly List<PlayerStatConfig> _statConfigs;
        public int UpgradePoints { get; private set; }
        public bool IsPlayerDead => _playerDeath.IsDead;
        
        public PlayerStatsWindowModel(PlayerStatsModel statsModel, PlayerStatsSaveLoad saveLoad, PlayerDeath playerDeath, 
            IGamePauseService pauseService, IScoreService scoreService, List<PlayerStatConfig> statConfigs)
        {
            _statsModel = statsModel;
            _saveLoad = saveLoad;
            _playerDeath = playerDeath;
            _pauseService = pauseService;
            _scoreService = scoreService;
            _statConfigs = statConfigs;
        }

        public void Initialize()
        {
            foreach (PlayerStatData statData in _statsModel.GetStats()) 
                statData.OnStatChanged += InvokeStatChanged;

            _scoreService.OnScoreAdded += AddUpgradePoints;
            
            UpgradePoints = _saveLoad.LoadStats().UpgradePoints;
        }

        public void Dispose()
        {
            foreach (PlayerStatData stat in GetStats())
                stat.OnStatChanged -= InvokeStatChanged;
            
            _scoreService.OnScoreAdded -= AddUpgradePoints;
        }

        public async UniTask ApplyChangesAsync()
        {
            if (!HasAnyChanges()) 
                return;
            
            foreach (PlayerStatData stat in GetStats()) 
                stat.ApplyPreviewLevel();
            
            await _saveLoad.SaveStatsAsync(UpgradePoints);
        }

        public void DiscardPreviewChanges()
        {
            if (!HasAnyChanges()) 
                return;
            
            int returnedPoints = 0;

            foreach (PlayerStatData stat in GetStats())
            {
                returnedPoints += stat.PreviewLevel - stat.Level;
                stat.DiscardPreviewLevel();
            }
               
            UpgradePoints += returnedPoints;
            OnStatsChanged?.Invoke();
        }

        public void UpgradeStat(StatName statName)
        {
            if (!CanUpgrade(statName))
                return;

            GetStat(statName).IncreasePreviewLevel();
            UpgradePoints--;
            OnStatsChanged?.Invoke();
        }

        public bool CanUpgrade(StatName statName)
        {
            if (UpgradePoints <=0 || !_statsModel.GetStatDictionary().ContainsKey(statName))
                return false;

            return GetStat(statName).PreviewLevel < GetStat(statName).MaxLevel;
        }

        public void SetPaused(bool paused) => 
            _pauseService.SetPaused(paused);

        public List<PlayerStatData> GetStats() => 
            _statsModel.GetStats();

        public PlayerStatData GetStat(StatName statName) => 
            _statsModel.GetStat(statName);

        public PlayerStatConfig FindStatConfigByName(StatName statName) => 
            _statConfigs.Find(statConfig => statConfig.Name == statName);

        private async void AddUpgradePoints(int points)
        {
            try
            {
                UpgradePoints += points;
                OnStatsChanged?.Invoke();
                await _saveLoad.SaveStatsAsync(UpgradePoints);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private bool HasAnyChanges() =>
            GetStats().Any(stat => stat.PreviewLevelHasChanged);

        private void InvokeStatChanged() => 
            OnStatsChanged?.Invoke();
    }
}
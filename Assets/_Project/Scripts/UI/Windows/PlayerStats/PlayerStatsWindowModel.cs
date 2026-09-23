using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services.GamePause;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowModel: IDisposable
    {
        public event Action OnStatsChanged;
        
        private readonly PlayerStatsModel _statsModel;
        private readonly PlayerStatsSaveLoad _saveLoad;
        private readonly PlayerDeath _playerDeath;
        private readonly IGamePauseService _pauseService;
        public int UpgradePoints { get; private set; }
        public bool IsPlayerDead => _playerDeath.IsDead;
        
        public PlayerStatsWindowModel(PlayerStatsModel statsModel, PlayerStatsSaveLoad saveLoad, 
            PlayerDeath playerDeath, IGamePauseService pauseService)
        {
            _statsModel = statsModel;
            _saveLoad = saveLoad;
            _playerDeath = playerDeath;
            _pauseService = pauseService;
        }

        public async UniTask Initialize()
        {
            foreach (PlayerStatData statData in _statsModel.GetStats()) 
                statData.OnStatChanged += InvokeStatChanged;

            PlayerStatsProgress progress = await _saveLoad.LoadStatsAsync();
            UpgradePoints = progress.UpgradePoints;
        }

        public void Dispose()
        {
            foreach (PlayerStatData stat in GetStats())
                stat.OnStatChanged -= InvokeStatChanged;
        }

        public async void ApplyChanges()
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

        public async UniTask AddUpgradePoint(int points = 1)
        {
            UpgradePoints += points;
            OnStatsChanged?.Invoke();
            await _saveLoad.SaveStatsAsync(UpgradePoints);
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

        private bool HasAnyChanges() =>
            GetStats().Any(stat => stat.PreviewLevelHasChanged);

        private void InvokeStatChanged() => 
            OnStatsChanged?.Invoke();
    }
}
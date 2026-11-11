using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Player.Components;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services;
using _Project.Scripts.Services.GamePause;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowModel : IDisposable
    {
        private readonly PlayerStatsModel _statsModel;
        private readonly PlayerStatsSaveLoad _saveLoad;
        private readonly IGamePauseService _pauseService;
        private readonly ScoreService _scoreService;
        private readonly LazyInject<PlayerContainer> _lazyPlayer;
        
        private  PlayerDeath _playerDeath;
        
        public int UpgradePoints { get; private set; }
        public bool IsPlayerDead => _playerDeath.IsDead;
        
        public PlayerStatsWindowModel(PlayerStatsModel statsModel, PlayerStatsSaveLoad saveLoad, IGamePauseService pauseService, 
            ScoreService scoreService, LazyInject<PlayerContainer> lazyPlayer)
        {
            _statsModel = statsModel;
            _saveLoad = saveLoad;
            _pauseService = pauseService;
            _scoreService = scoreService;
            _lazyPlayer = lazyPlayer;
        }

        public void Initialize()
        {
            _playerDeath = _lazyPlayer.Value.PlayerDeath;
            
            _scoreService.OnScoreAdded += AddUpgradePoints;
            _playerDeath.OnDied += SaveStats;
            
            UpgradePoints = _saveLoad.LoadStats().UpgradePoints;
        }
        
        public void Dispose()
        {
            _scoreService.OnScoreAdded -= AddUpgradePoints;
            _playerDeath.OnDied -= SaveStats;
        }

        public void ApplyStatUpgrades()
        {
            if (!_statsModel.HasAnyChanges()) 
                return;
            
            foreach (PlayerStatData stat in GetStats()) 
                stat.ApplyPreviewLevel();
        }

        public void DiscardPreviewChanges()
        {
            if (!_statsModel.HasAnyChanges()) 
                return;
            
            int returnedPoints = 0;

            foreach (PlayerStatData stat in GetStats())
            {
                returnedPoints += stat.PreviewLevel - stat.Level;
                stat.DiscardPreviewLevel();
            }
               
            UpgradePoints += returnedPoints;
        }

        public void PreviewStatUpgrade(StatName statName)
        {
            if (!CanUpgrade(statName))
                return;

            UpgradePoints--;
            PlayerStatData stat = _statsModel.GetStat(statName);
            stat.IncreasePreviewLevel();
        }

        public bool CanUpgrade(StatName statName)
        {
            bool statExist = _statsModel.TryGetStat(statName, out PlayerStatData stat);
            
            if (UpgradePoints <=0 || !statExist)
                return false;
            
            return stat.PreviewLevel < stat.MaxLevel;
        }

        private  void AddUpgradePoints(int points)
        {
            UpgradePoints += points;
            _saveLoad.UpdateStatsProgress(UpgradePoints);
        }

        public void SetPaused(bool paused) => 
            _pauseService.SetPaused(paused);

        public IReadOnlyList<PlayerStatData> GetStats() => 
            _statsModel.GetStats();

        public PlayerStatData GetStat(StatName statName) => 
            _statsModel.GetStat(statName);

        public PlayerStatConfig FindStatConfigByName(StatName statName) => 
            _statsModel.FindStatConfigByName(statName);

        public async UniTask SaveStatsAsync()
        {
            _saveLoad.UpdateStatsProgress(UpgradePoints);
            await _saveLoad.SaveStatsAsync();
        }
        
        private void SaveStats() => 
            SaveStatsAsync().Forget();
    }
}
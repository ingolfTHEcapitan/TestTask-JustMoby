using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services;
using _Project.Scripts.Services.GamePause;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowModel : IDisposable
    {
        public event Action OnStatsChanged;
        
        private readonly PlayerStatsModel _statsModel;
        private readonly PlayerStatsSaveLoad _saveLoad;
        private readonly PlayerDeath _playerDeath;
        private readonly IGamePauseService _pauseService;
        private readonly ScoreService _scoreService;
        private readonly List<PlayerStatConfig> _statConfigs;
        public int UpgradePoints { get; private set; }
        public bool IsPlayerDead => _playerDeath.IsDead;
        
        public PlayerStatsWindowModel(PlayerStatsModel statsModel, PlayerStatsSaveLoad saveLoad, PlayerDeath playerDeath, 
            IGamePauseService pauseService, ScoreService scoreService, List<PlayerStatConfig> statConfigs)
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
            _playerDeath.OnDied += UpdateStatsProgressAndSave;
            
            UpgradePoints = _saveLoad.LoadStats().UpgradePoints;
        }

       

        public void Dispose()
        {
            foreach (PlayerStatData stat in GetStats())
                stat.OnStatChanged -= InvokeStatChanged;
            
            _scoreService.OnScoreAdded -= AddUpgradePoints;
            _playerDeath.OnDied -= UpdateStatsProgressAndSave;
        }

        public async UniTask ApplyChangesAsync()
        {
            if (!HasAnyChanges()) 
                return;
            
            foreach (PlayerStatData stat in GetStats()) 
                stat.ApplyPreviewLevel();
            
            await UpdateStatsProgressAndSaveAsync();
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

            _statsModel.TryGetStat(statName, out PlayerStatData stat);
            stat.IncreasePreviewLevel();
            UpgradePoints--;
            OnStatsChanged?.Invoke();
        }

        public bool CanUpgrade(StatName statName)
        {
            if (UpgradePoints <=0 || !_statsModel.TryGetStat(statName, out _))
                return false;

            _statsModel.TryGetStat(statName, out PlayerStatData stat);
            
            return stat.PreviewLevel < stat.MaxLevel;
        }

        public void SetPaused(bool paused) => 
            _pauseService.SetPaused(paused);

        public IReadOnlyList<PlayerStatData> GetStats() => 
            _statsModel.GetStats();

        public bool TryGetStat(StatName statName, out PlayerStatData stat) => 
            _statsModel.TryGetStat(statName, out stat);

        public PlayerStatConfig FindStatConfigByName(StatName statName) => 
            _statConfigs.Find(statConfig => statConfig.Name == statName);

        private  void AddUpgradePoints(int points)
        {
            UpgradePoints += points;
            OnStatsChanged?.Invoke();
            _saveLoad.UpdateStatsProgress(UpgradePoints);
        }

        private bool HasAnyChanges() =>
            GetStats().Any(stat => stat.PreviewLevelHasChanged);

        private void InvokeStatChanged() => 
            OnStatsChanged?.Invoke();

        private async UniTask UpdateStatsProgressAndSaveAsync()
        {
            _saveLoad.UpdateStatsProgress(UpgradePoints);
            await _saveLoad.SaveStatAsync();
        }
        
        private void UpdateStatsProgressAndSave() => 
            UpdateStatsProgressAndSaveAsync().Forget();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Configs;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.Logic.Player.Stats
{
    public class PlayerStatsModel: IDisposable
    {
        public event Action OnStatsChanged;
        
        private readonly ISaveLoadService _saveLoadService;
        private readonly List<PlayerStatConfig> _configs;
        private IAssetProvider _assetProvider;
        private IUIFactory _uiFactory;
        private IProgressService _progressService;

        public Dictionary<StatName, PlayerStatData> Stats { get; private set; } = new Dictionary<StatName, PlayerStatData>();
        public int UpgradePoints { get; private set; }

        public PlayerStatsModel([Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, IProgressService progressService, 
            IUIFactory uiFactory, List<PlayerStatConfig> configs)
        {
            _saveLoadService = saveLoadService;
            _progressService = progressService;
            _uiFactory = uiFactory;
            _configs = configs;
        }

        public async UniTask Initialize()
        {
            foreach (var config in _configs)
            {
                PlayerStatData statData = new PlayerStatData(config);
                await statData.LoadUIPartsAsync(config, _uiFactory);
                statData.OnStatChanged += InvokeStatChanged;
                Stats[config.Name] = statData;
            }

            await LoadStats();
        }

        public void Dispose()
        {
            foreach (PlayerStatData stat in Stats.Values)
                stat.OnStatChanged -= InvokeStatChanged;
        }

        public void ApplyChanges()
        {
            if (!HasAnyChanges()) 
                return;
            
            foreach (PlayerStatData stat in Stats.Values) 
                stat.ApplyPreviewLevel();
            
            SaveStats();
        }

        public void DiscardPreviewChanges()
        {
            if (!HasAnyChanges()) 
                return;
            
            int returnedPoints = 0;

            foreach (PlayerStatData stat in Stats.Values)
            {
                returnedPoints += stat.PreviewLevel - stat.Level;
                stat.DiscardPreviewLevel();
            }
               
            UpgradePoints += returnedPoints;
            OnStatsChanged?.Invoke();
        }

        public void AddUpgradePoint(int points = 1)
        {
            UpgradePoints += points;
            OnStatsChanged?.Invoke();
            SaveStats();
        }

        public void UpgradeStat(StatName statName)
        {
            if (!CanUpgrade(statName))
                return;

            Stats[statName].IncreasePreviewLevel();
            UpgradePoints--;
            OnStatsChanged?.Invoke();
        }

        public float GetStatValue(StatName statName)
        {
            if (Stats.TryGetValue(statName, out PlayerStatData stat))
                return stat.CurrentValue;
            
            return 0;
        }

        public List<PlayerStatData> GetStats() => 
            new List<PlayerStatData>(Stats.Values);
        
        public PlayerStatData GetStat(StatName statName) => 
            Stats[statName];

        public bool CanUpgrade(StatName statName)
        {
            if (UpgradePoints <=0 || !Stats.ContainsKey(statName))
                return false;

            return Stats[statName].PreviewLevel < Stats[statName].MaxLevel;
        }
        
        private bool HasAnyChanges() =>
            Stats.Values.Any(stat => stat.PreviewLevelHasChanged);

        private async UniTask LoadStats()
        {
            PlayerProgress playerProgress = await _saveLoadService.LoadProgressAsync();
            PlayerStatsData progress = playerProgress.PlayerStatsData;
            
            UpgradePoints = progress.UpgradePoints;

            if (Stats.ContainsKey(StatName.Health))
                Stats[StatName.Health].SetLevel(progress.HealthLevel);
            
            if (Stats.ContainsKey(StatName.Speed))
                Stats[StatName.Speed].SetLevel(progress.SpeedLevel);
            
            if (Stats.ContainsKey(StatName.Damage))
                Stats[StatName.Damage].SetLevel(progress.DamageLevel);
        }

        private async UniTask SaveStats()
        {
            PlayerStatsData progress = _progressService.PlayerProgress.PlayerStatsData;

            progress.UpgradePoints = UpgradePoints;
            progress.HealthLevel = Stats.TryGetValue(StatName.Health, out PlayerStatData health) ? health.Level : 0;
            progress.SpeedLevel = Stats.TryGetValue(StatName.Speed, out PlayerStatData speed) ? speed.Level : 0;
            progress.DamageLevel = Stats.TryGetValue(StatName.Damage, out PlayerStatData damage) ? damage.Level : 0;
            
            await _saveLoadService.SaveProgressAsync(_progressService);
        }
        
        private void InvokeStatChanged() => 
            OnStatsChanged?.Invoke();
    }
}
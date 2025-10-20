using System;
using System.Collections.Generic;
using _Project._Scripts.Configs;
using _Project._Scripts.Data;
using _Project._Scripts.Infrastructure.Services.ConfigsManagement;
using _Project._Scripts.Infrastructure.Services.SaveLoad;
using UnityEngine;

namespace _Project._Scripts.Logic.PlayerStats
{
    public class PlayerStatsModel: IDisposable
    {
        public event Action OnStatsChanged;
        
        private ISaveLoadService _saveLoadService;
        private IConfigsProvider _configs;

        public Dictionary<StatName, PlayerStatData> Stats { get; private set; } = new Dictionary<StatName, PlayerStatData>();
        public int UpgradePoints { get; private set; }
        
        public void Construct(ISaveLoadService saveLoadService, IConfigsProvider configs)
        {
            _saveLoadService = saveLoadService;
            _configs = configs;
        }

        public void Initialize()
        {
            foreach (PlayerStatConfig config in _configs.PlayerStats)
            {
                PlayerStatData statData = new PlayerStatData(config);
                statData.OnStatChanged += InvokeStatChanged;
                Stats[config.Name] = statData;
            }

            LoadStats();
        }

        public void Dispose()
        {
            foreach (PlayerStatData stat in Stats.Values)
                stat.OnStatChanged -= InvokeStatChanged;
        }

        public void ApplyChanges()
        {
            foreach (PlayerStatData stat in Stats.Values) 
                stat.ApplyPreviewLevel();
            
            SaveStats();
        }

        public void DiscardPreviewChanges()
        {
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

        public bool CanUpgrade(StatName statName)
        {
            if (UpgradePoints <=0 || !Stats.ContainsKey(statName))
                return false;

            return Stats[statName].PreviewLevel < Stats[statName].MaxLevel;
        }
        
        private void LoadStats()
        {
            PlayerProgress progress = _saveLoadService.LoadProgress();
            if (progress == null)
            {
                foreach (PlayerStatData stat in Stats.Values)
                    stat.RecalculateCurrentValue();

                UpgradePoints = 0;
                SaveStats();
                return;
            }
            
            UpgradePoints = progress.UpgradePoints;

            if (Stats.ContainsKey(StatName.Health))
                Stats[StatName.Health].SetLevel(progress.HealthLevel);
            
            if (Stats.ContainsKey(StatName.Speed))
                Stats[StatName.Speed].SetLevel(progress.SpeedLevel);
            
            if (Stats.ContainsKey(StatName.Damage))
                Stats[StatName.Damage].SetLevel(progress.DamageLevel);
            
        }

        private void SaveStats()
        {
            PlayerProgress progress = new PlayerProgress
            {
                UpgradePoints = UpgradePoints,
                HealthLevel = Stats.TryGetValue(StatName.Health, out PlayerStatData health) ? health.Level : 0,
                SpeedLevel = Stats.TryGetValue(StatName.Speed, out PlayerStatData speed) ? speed.Level : 0,
                DamageLevel = Stats.TryGetValue(StatName.Damage, out PlayerStatData damage) ? damage.Level : 0
            };
            
            _saveLoadService.SaveProgress(progress);
        }
        
        private void InvokeStatChanged() => 
            OnStatsChanged?.Invoke();
    }
}
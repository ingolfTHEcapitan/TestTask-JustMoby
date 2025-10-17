using System;
using System.Collections.Generic;
using _Project._Scripts.Configs;
using _Project._Scripts.Data;
using _Project._Scripts.Infrastructure.Services.SaveLoad;
using UnityEngine;

namespace _Project._Scripts.Logic.StatSystem
{
    public class PlayerStatsSystem: MonoBehaviour
    {
        public event Action OnStatsChanged;
        
        [SerializeField] private List<PlayerStatConfig> _statConfigs;
        
        private ISaveLoadService _saveLoadService;
        
        public Dictionary<StatName, PlayerStat> Stats { get; private set; } = new Dictionary<StatName, PlayerStat>();
        public int UpgradePoints { get; private set; }
        
        
        public void Construct(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }

        public void Initialize()
        {
            foreach (PlayerStatConfig config in _statConfigs)
            {
                PlayerStat stat = new PlayerStat(config);
                stat.OnStatChanged += InvokeStatChanged;
                Stats[config.Name] = stat;
            }

            LoadStats();
        }

        private void OnDestroy()
        {
            foreach (PlayerStat stat in Stats.Values)
                stat.OnStatChanged -= InvokeStatChanged;
        }

        public void ApplyChanges()
        {
            foreach (PlayerStat stat in Stats.Values) 
                stat.ApplyPreviewLevel();
            
            SaveStats();
        }

        public void DiscardPreviewChanges()
        {
            int returnedPoints = 0;

            foreach (PlayerStat stat in Stats.Values)
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
            if (Stats.TryGetValue(statName, out PlayerStat stat))
                return stat.CurrentValue;
            
            return 0;
        }
        
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
                foreach (PlayerStat stat in Stats.Values)
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
                HealthLevel = Stats.TryGetValue(StatName.Health, out PlayerStat health) ? health.Level : 0,
                SpeedLevel = Stats.TryGetValue(StatName.Speed, out PlayerStat speed) ? speed.Level : 0,
                DamageLevel = Stats.TryGetValue(StatName.Damage, out PlayerStat damage) ? damage.Level : 0
            };
            
            _saveLoadService.SaveProgress(progress);
        }


        private void InvokeStatChanged() => 
            OnStatsChanged?.Invoke();
    }
}
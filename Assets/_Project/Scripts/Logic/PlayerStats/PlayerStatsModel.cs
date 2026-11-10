using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Configs;

namespace _Project.Scripts.Logic.PlayerStats
{
    public class PlayerStatsModel
    {
        private readonly List<PlayerStatConfig> _configs;
        private readonly Dictionary<StatName, PlayerStatData> _stats = new Dictionary<StatName, PlayerStatData>();

        public PlayerStatsModel(List<PlayerStatConfig> configs) => 
            _configs = configs;

        public void CreateStats()
        {
            foreach (PlayerStatConfig config in _configs)
            {
                PlayerStatData statData = new PlayerStatData(config);
                _stats[config.Name] = statData;
            }
        }
        
        public bool HasAnyChanges() =>
            _stats.Values.Any(stat => stat.PreviewLevelHasChanged);

        public PlayerStatConfig FindStatConfigByName(StatName statName) => 
            _configs.Find(statConfig => statConfig.Name == statName);

        public IReadOnlyList<PlayerStatData> GetStats() => 
            new List<PlayerStatData>(_stats.Values);

        public float GetStatValue(StatName statName) => 
            GetStat(statName).CurrentValue;

        public PlayerStatData GetStat(StatName statName)
        {
            if (_stats.TryGetValue(statName, out PlayerStatData stat))
                return stat;
            
            throw new KeyNotFoundException($"[PlayerStatsModel] Stat not found for name : {statName}");
        }
        
        public bool TryGetStat(StatName statName, out PlayerStatData stat) => 
            _stats.TryGetValue(statName, out stat);
    }
}
using System.Collections.Generic;
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
        
        public float GetStatValue(StatName statName)
        {
            if (_stats.TryGetValue(statName, out PlayerStatData stat))
                return stat.CurrentValue;
            
            return 0;
        }
        
        public IReadOnlyList<PlayerStatData> GetStats() => 
            new List<PlayerStatData>(_stats.Values);
        
        public bool TryGetStat(StatName statName, out PlayerStatData stat) => 
            _stats.TryGetValue(statName, out stat);
    }
}
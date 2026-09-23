using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Logic.PlayerStats
{
    public class PlayerStatsModel
    {
        private readonly List<PlayerStatConfig> _configs;
        private readonly IUIFactory _uiFactory;
        private readonly Dictionary<StatName, PlayerStatData> _stats = new Dictionary<StatName, PlayerStatData>();

        public PlayerStatsModel(IUIFactory uiFactory, List<PlayerStatConfig> configs)
        {
            _uiFactory = uiFactory;
            _configs = configs;
        }
        
        public async UniTask CreateStatsAsync()
        {
            foreach (PlayerStatConfig config in _configs)
            {
                PlayerStatData statData = new PlayerStatData(config);
                await statData.LoadUIPartsAsync(config, _uiFactory);
                _stats[config.Name] = statData;
            }
        }
        
        public float GetStatValue(StatName statName)
        {
            if (_stats.TryGetValue(statName, out PlayerStatData stat))
                return stat.CurrentValue;
            
            return 0;
        }
        
        public List<PlayerStatData> GetStats() => 
            new List<PlayerStatData>(_stats.Values);
        
        public PlayerStatData GetStat(StatName statName) => 
            _stats[statName];
        public Dictionary<StatName, PlayerStatData> GetStatDictionary() => 
            _stats;
    }
}
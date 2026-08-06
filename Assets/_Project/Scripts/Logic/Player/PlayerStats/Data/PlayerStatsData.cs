using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Logic.Player.PlayerStats.Data
{
    public class PlayerStatsData
    {
        private readonly List<PlayerStatConfig> _configs;
        private readonly IUIFactory _uiFactory;
        private readonly Dictionary<StatName, PlayerStatData> _stats = new Dictionary<StatName, PlayerStatData>();

        public PlayerStatsData(IUIFactory uiFactory, List<PlayerStatConfig> configs)
        {
            _uiFactory = uiFactory;
            _configs = configs;
        }
        
        public async UniTask<Dictionary<StatName, PlayerStatData>> CreateStatsAsync()
        {
            foreach (var config in _configs)
            {
                PlayerStatData statData = new PlayerStatData(config);
                await statData.LoadUIPartsAsync(config, _uiFactory);
                _stats[config.Name] = statData;
            }
            
            return _stats;
        }
        
        public float GetStatValue(StatName statName)
        {
            if (_stats.TryGetValue(statName, out PlayerStatData stat))
                return stat.CurrentValue;
            
            return 0;
        }
        
        public List<PlayerStatData> GetStatValues() => 
            new List<PlayerStatData>(_stats.Values);
        
        public PlayerStatData GetStat(StatName statName) => 
            _stats[statName];
        public Dictionary<StatName, PlayerStatData> GetStats() => 
            _stats;
    }
}
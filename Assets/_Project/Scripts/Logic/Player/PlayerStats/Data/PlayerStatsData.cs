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
        
        public Dictionary<StatName, PlayerStatData> Stats { get; private set; } = new Dictionary<StatName, PlayerStatData>();

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
                Stats[config.Name] = statData;
            }
            
            return Stats;
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
    }
}
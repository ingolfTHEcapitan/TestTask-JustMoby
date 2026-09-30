using System.Collections.Generic;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.Logic.PlayerStats
{
    public class PlayerStatsSaveLoad
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IProgressService _progressService;
        private readonly Dictionary<StatName, PlayerStatData> _stats;

        private PlayerStatsProgress Progress => _progressService.PlayerProgress.PlayerStatsProgress;

        public PlayerStatsSaveLoad(PlayerStatsModel statsModel, [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, 
            IProgressService progressService)
        {
            _stats = statsModel.GetStatDictionary();
            _saveLoadService = saveLoadService;
            _progressService = progressService;
        }
        
        public PlayerStatsProgress LoadStats()
        {
            if (_stats.TryGetValue(StatName.Health, out PlayerStatData health))
                health.SetLevel(Progress.HealthLevel);
           
            if (_stats.TryGetValue(StatName.Speed, out PlayerStatData speed))
                speed.SetLevel(Progress.SpeedLevel);
           
            if (_stats.TryGetValue(StatName.Damage, out PlayerStatData damage))
                damage.SetLevel(Progress.DamageLevel);
            
            return Progress;
        }

        public async UniTask SaveStatsAsync(int upgradePoints)
        {
            Progress.UpgradePoints = upgradePoints;
            Progress.HealthLevel = _stats.TryGetValue(StatName.Health, out PlayerStatData health) ? health.Level : 0;
            Progress.SpeedLevel = _stats.TryGetValue(StatName.Speed, out PlayerStatData speed) ? speed.Level : 0;
            Progress.DamageLevel = _stats.TryGetValue(StatName.Damage, out PlayerStatData damage) ? damage.Level : 0;
            
            await _saveLoadService.SaveProgressAsync(_progressService);
        }
    }
}
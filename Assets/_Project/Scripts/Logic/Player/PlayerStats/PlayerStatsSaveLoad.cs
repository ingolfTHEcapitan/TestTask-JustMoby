using System.Collections.Generic;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.Logic.Player.PlayerStats
{
    public class PlayerStatsSaveLoad
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IProgressService _progressService;
        private readonly Dictionary<StatName, PlayerStatData> _stats;

        public PlayerStatsSaveLoad(PlayerStatsData statsData, [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, 
            IProgressService progressService)
        {
            _stats = statsData.Stats;
            _saveLoadService = saveLoadService;
            _progressService = progressService;
        }
        
        public async UniTask<PlayerStatsProgress> LoadStats()
        {
            PlayerProgress playerProgress = await _saveLoadService.LoadProgressAsync();
            PlayerStatsProgress progress = playerProgress.PlayerStatsProgress;
            
            if (_stats.TryGetValue(StatName.Health, out PlayerStatData health))
                health.SetLevel(progress.HealthLevel);
           
            if (_stats.TryGetValue(StatName.Speed, out PlayerStatData speed))
                speed.SetLevel(progress.SpeedLevel);
           
            if (_stats.TryGetValue(StatName.Damage, out PlayerStatData damage))
                damage.SetLevel(progress.DamageLevel);
            
            return progress;
        }

        public async UniTask SaveStats(int upgradePoints)
        {
            PlayerStatsProgress progress = _progressService.PlayerProgress.PlayerStatsProgress;

            progress.UpgradePoints = upgradePoints;
            progress.HealthLevel = _stats.TryGetValue(StatName.Health, out PlayerStatData health) ? health.Level : 0;
            progress.SpeedLevel = _stats.TryGetValue(StatName.Speed, out PlayerStatData speed) ? speed.Level : 0;
            progress.DamageLevel = _stats.TryGetValue(StatName.Damage, out PlayerStatData damage) ? damage.Level : 0;
            
            await _saveLoadService.SaveProgressAsync(_progressService);
        }
    }
}
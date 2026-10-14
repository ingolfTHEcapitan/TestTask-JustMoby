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
        private readonly PlayerStatsModel _statsModel;

        private PlayerStatsProgress Progress => _progressService.PlayerProgress.PlayerStatsProgress;

        public PlayerStatsSaveLoad(PlayerStatsModel statsModel, [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, 
            IProgressService progressService)
        {
            _statsModel = statsModel;
            _saveLoadService = saveLoadService;
            _progressService = progressService;
        }
        
        public PlayerStatsProgress LoadStats()
        {
            if (_statsModel.TryGetStat(StatName.Health, out PlayerStatData health))
                health.SetLevel(Progress.HealthLevel);
           
            if (_statsModel.TryGetStat(StatName.Speed, out PlayerStatData speed))
                speed.SetLevel(Progress.SpeedLevel);
           
            if (_statsModel.TryGetStat(StatName.Damage, out PlayerStatData damage))
                damage.SetLevel(Progress.DamageLevel);
            
            return Progress;
        }

        public void UpdateStatsProgress(int upgradePoints)
        {
            Progress.UpgradePoints = upgradePoints;
            Progress.HealthLevel = _statsModel.TryGetStat(StatName.Health, out PlayerStatData health) ? health.Level : 0;
            Progress.SpeedLevel = _statsModel.TryGetStat(StatName.Speed, out PlayerStatData speed) ? speed.Level : 0;
            Progress.DamageLevel = _statsModel.TryGetStat(StatName.Damage, out PlayerStatData damage) ? damage.Level : 0;
        }

        public async UniTask SaveStatAsync() => 
            await _saveLoadService.SaveProgressAsync(_progressService);
    }
}
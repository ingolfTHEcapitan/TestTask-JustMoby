using System;
using _Project.Scripts.UI.Windows.PlayerStats;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.UpgradePoints
{
    public class UpgradePointsService : IUpgradePointsService
    {
        public event Action OnPointAdded;

        private readonly PlayerStatsWindowModel _playerStatsWindowModel;
        
        public int CurrentPoints { get; private set; }

        public UpgradePointsService(PlayerStatsWindowModel playerStatsWindowModel) => 
            _playerStatsWindowModel = playerStatsWindowModel;

        public async UniTask AddPointAsync()
        {
            await _playerStatsWindowModel.AddUpgradePoint();
            CurrentPoints = _playerStatsWindowModel.UpgradePoints;
            OnPointAdded?.Invoke();
        }
    }
}
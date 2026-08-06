using System;
using _Project.Scripts.Logic.Player.PlayerStats;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.UpgradePoints
{
    public class UpgradePointsService : IUpgradePointsService
    {
        public event Action OnPointAdded;

        private readonly PlayerStatsModel _playerStatsModel;
        
        public int CurrentPoints { get; private set; }

        public UpgradePointsService(PlayerStatsModel playerStatsModel) => 
            _playerStatsModel = playerStatsModel;

        public async UniTask AddPointAsync()
        {
            await _playerStatsModel.AddUpgradePoint();
            CurrentPoints = _playerStatsModel.UpgradePoints;
            OnPointAdded?.Invoke();
        }
    }
}
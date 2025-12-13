using System;
using _Project.Scripts.Logic.PlayerStats;

namespace _Project.Scripts.Services.Score
{
    public class ScoreService : IScoreService
    {
        public event Action OnScoreChanged;

        private readonly PlayerStatsModel _playerStatsModel;
        
        public int CurrentScore { get; private set; }

        public ScoreService(PlayerStatsModel playerStatsModel) => 
            _playerStatsModel = playerStatsModel;

        public void AddScore()
        {
            _playerStatsModel.AddUpgradePoint();
            CurrentScore = _playerStatsModel.UpgradePoints;
            OnScoreChanged?.Invoke();
        }
    }
}
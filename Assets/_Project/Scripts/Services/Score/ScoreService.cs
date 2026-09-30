using System;

namespace _Project.Scripts.Services.Score
{
    public class ScoreService : IScoreService
    {
        public event Action<int> OnScoreAdded;
        public event Action OnScoreChanged;

        public int CurrentScore { get; private set; }
        
        public void Add(int value = 1)
        {
            CurrentScore += value;
            OnScoreAdded?.Invoke(value);
            OnScoreChanged?.Invoke();
        }
    }
}
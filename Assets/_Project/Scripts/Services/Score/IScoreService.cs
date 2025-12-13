using System;

namespace _Project.Scripts.Services.Score
{
    public interface IScoreService
    {
        event Action OnScoreChanged;
        int CurrentScore { get; }
        void AddScore();
    }
}
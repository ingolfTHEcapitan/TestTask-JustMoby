namespace _Project.Scripts.Services.Analytics
{
    public interface IAnalyticsService
    {
        void LogGameStart();
        void LogGameEnd(int shotsFired, int enemiesKilled);
        void LogPlayerRevive(int reviveCount);
    }
}
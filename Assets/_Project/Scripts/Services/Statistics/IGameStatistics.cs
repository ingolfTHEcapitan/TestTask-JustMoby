namespace _Project.Scripts.Services.Statistics
{
    public interface IGameStatistics
    {
        int ShotsFired { get; }
        int EnemiesKilled { get; }
        int ReviveCount { get; }
        void RecordShot();
        void RecordEnemyKilled();
        void RecordRevive();
    }
}
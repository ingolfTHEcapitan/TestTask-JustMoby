using System;

namespace _Project.Scripts.Services.Statistics
{
    public class GameStatistics: IGameStatistics, IDisposable
    {
        public int ShotsFired { get; private set; }
        public int EnemiesKilled { get; private set; }
        public int ReviveCount { get; private set; }
        
        public void RecordShot() =>
            ShotsFired++;
        
        public void RecordEnemyKilled() =>
            EnemiesKilled++;
        
        public void RecordRevive() =>
            ReviveCount++;

        public void Dispose() => 
            Reset();

        private void Reset()
        {
            ShotsFired = 0;
            EnemiesKilled = 0;
            ReviveCount = 0;
        }
    }
}
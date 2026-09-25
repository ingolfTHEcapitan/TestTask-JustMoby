using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace _Project.Scripts.Services.Analytics
{
    public class FirebaseAnalyticsService : IAnalyticsService
    {
        private const string EventGameEnd = "game_end";
        private const string EventPlayerRevive = "player_revive";
        private const string ParameterShotsFired = "shots_fired";
        private const string ParameterEnemiesKilled = "enemies_killed";
        private const string ParameterReviveCount = "revive_count";
        
        public async UniTask InitializeAsync()
        {
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();

            if (dependencyStatus != DependencyStatus.Available)
                Debug.LogError($"[FIREBASE ANALYTICS] Can't resolve all Firebase dependencies: {dependencyStatus}");
        }

        public void LogGameStart() => 
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart);

        public void LogGameEnd(int shotsFired, int enemiesKilled)
        {
            FirebaseAnalytics.LogEvent(EventGameEnd, 
                new Parameter(ParameterShotsFired, shotsFired),
                new Parameter(ParameterEnemiesKilled, enemiesKilled));
        }
        
        public void LogPlayerRevive(int reviveCount)
        {
            FirebaseAnalytics.LogEvent(EventPlayerRevive, 
                new Parameter(ParameterReviveCount, reviveCount));
        }
    }
}
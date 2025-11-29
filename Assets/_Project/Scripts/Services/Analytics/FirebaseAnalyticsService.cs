using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
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

        private bool _isFirebaseReady;
        private FirebaseApp app;
        
        public FirebaseAnalyticsService() => 
            Initialize();

        private void Initialize()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => 
            {
                DependencyStatus dependencyStatus = task.Result;
                
                if (dependencyStatus == DependencyStatus.Available) 
                {
                    app = FirebaseApp.DefaultInstance;
                    _isFirebaseReady = true;
                } else 
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }

        public void LogGameStart()
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart);
            Debug.Log("EventGameEnd");
        }

        public void LogGameEnd(int shotsFired, int enemiesKilled)
        {
            FirebaseAnalytics.LogEvent(EventGameEnd, 
                new Parameter(ParameterShotsFired, shotsFired),
                new Parameter(ParameterEnemiesKilled, enemiesKilled));

            Debug.Log($" GameEnd: {ParameterShotsFired}: {shotsFired}, {ParameterEnemiesKilled}: {enemiesKilled}");
        }
        
        public void LogPlayerRevive(int reviveCount)
        {
            FirebaseAnalytics.LogEvent(EventPlayerRevive, 
                new Parameter(ParameterReviveCount, reviveCount));
            
            Debug.Log("EventPlayerRevive");
        }
    }
}
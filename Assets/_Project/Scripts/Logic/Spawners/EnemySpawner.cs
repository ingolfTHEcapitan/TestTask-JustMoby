using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Logic.Enemy;
using _Project.Scripts.Services.Factory.EnemyFactory;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.Statistics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Logic.Spawners
{
    public class EnemySpawner: IDisposable
    {
        private readonly IGamePauseService _pauseService;
        private readonly IEnemyFactory _factory;
        private readonly IGameStatistics _statistics;
        private readonly List<EnemyDeath> _spawnedEnemies = new List<EnemyDeath>();
        private readonly EnemySpawnerConfig _config;
        private CancellationTokenSource _cancellationTokenSource;

        public EnemySpawner(EnemySpawnerConfig config, IGamePauseService pauseService, IEnemyFactory factory, 
            IGameStatistics statistics)
        {
            _pauseService = pauseService;
            _factory = factory;
            _statistics = statistics;
            _config = config;
        }
        
        public void SpawnAround(Transform target, Transform playerTransform)
        {
            StopSpawning();
            _cancellationTokenSource = new CancellationTokenSource();
            SpawnAroundAsync(target, playerTransform, _cancellationTokenSource.Token).Forget();
        }

        public void KillAllEnemies()
        {
            foreach (EnemyDeath enemy in _spawnedEnemies) 
                enemy.KillEnemy();
        }
        
        public void StopSpawning()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        public void Dispose() => 
            StopSpawning();

        private async UniTaskVoid SpawnAroundAsync(Transform target, Transform playerTransform, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                
                await UniTask.WaitWhile(() => _pauseService.IsPaused, cancellationToken: token);
                
                if (_spawnedEnemies.Count < _config.EnemiesAtTime)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_config.SpawnDelay), cancellationToken: token);
                    await InitEnemy(target, playerTransform);
                }
                
                await UniTask.WaitUntil(() => _spawnedEnemies.Count < _config.EnemiesAtTime, cancellationToken: token);
            }
        }

        private async Task InitEnemy(Transform target, Transform playerTransform)
        {
            EnemyDeath enemyDeath = await _factory.CreateEnemy(_config, GetSpawnPosition(target), playerTransform);
            enemyDeath.OnDied += OnEnemyDeath;
            _spawnedEnemies.Add(enemyDeath);
        }

        private Vector3 GetSpawnPosition(Transform target)
        {
            Vector2 spawnDirection = Random.insideUnitCircle * _config.SpawnDistance;
            Vector3 offset = new Vector3(spawnDirection.x, 0, spawnDirection.y);
            Vector3 spawnPosition = target.position + offset;
            return spawnPosition;
        }

        private void OnEnemyDeath(EnemyDeath enemyDeath)
        {
            enemyDeath.OnDied -= OnEnemyDeath;
            _spawnedEnemies.Remove(enemyDeath);
            _statistics.RecordEnemyKilled();
        }
    }
}
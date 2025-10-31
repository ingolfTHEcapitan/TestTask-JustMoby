using System;
using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Enemy;
using _Project.Scripts.Infrastructure.Services.Factory.EnemyFactory;
using _Project.Scripts.Infrastructure.Services.GamePause;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Logic.Spawners
{
    public class EnemySpawner: IDisposable
    {
        private readonly IGamePauseService _pauseService;
        private readonly IEnemyFactory _factory;
        private readonly List<EnemyDeath> _spawnedEnemies = new List<EnemyDeath>();
        private readonly EnemySpawnerConfig _config;
        private CancellationTokenSource _cancellationTokenSource;

        public EnemySpawner(EnemySpawnerConfig config, IGamePauseService pauseService, IEnemyFactory factory)
        {
            _pauseService = pauseService;
            _factory = factory;
            _config = config;
        }
        
        public void SpawnAround(Transform target)
        {
            StopSpawning();
            _cancellationTokenSource = new CancellationTokenSource();
            SpawnAroundAsync(target, _cancellationTokenSource.Token).Forget();
        }

        public void StopSpawning()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        public void Dispose() => 
            StopSpawning();

        private async UniTaskVoid SpawnAroundAsync(Transform target, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                
                await UniTask.WaitWhile(() => _pauseService.IsPaused, cancellationToken: token);
                
                if (_spawnedEnemies.Count < _config.EnemiesAtTime)
                {
                    InitEnemy(target);
                    await UniTask.Delay(TimeSpan.FromSeconds(_config.SpawnDelay), cancellationToken: token);
                }
                
                await UniTask.WaitUntil(() => _spawnedEnemies.Count < _config.EnemiesAtTime, cancellationToken: token);
            }
        }

        private void InitEnemy(Transform target)
        {
            EnemyDeath enemyDeath = _factory.CreateEnemy(_config, GetSpawnPosition(target));
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
        }
    }
}
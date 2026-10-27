using System;
using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Logic.Enemy;
using _Project.Scripts.Services.GamePause;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Logic.Spawners
{
    public class EnemySpawner : IDisposable
    {
        private readonly IGamePauseService _pauseService;
        private readonly EnemyFactory _factory;
        private readonly List<EnemyContainer> _spawnedEnemies = new List<EnemyContainer>();
        private readonly EnemySpawnerConfig _config;
        
        private CancellationTokenSource _cancellationTokenSource;

        public EnemySpawner(IGamePauseService pauseService, EnemyFactory factory, EnemySpawnerConfig config)
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

        public void KillAllEnemies()
        {
            foreach (EnemyContainer enemy in _spawnedEnemies) 
                enemy.EnemyDeath.KillEnemy();
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
                await UniTask.WaitUntil(() => _spawnedEnemies.Count < _config.EnemiesAtTime, cancellationToken: token);
                
                await PausableDelayAsync(_config.SpawnDelay, token);
              
                if (_spawnedEnemies.Count < _config.EnemiesAtTime) 
                    await InitEnemyAsync(target);
            }
        }

        private async UniTask PausableDelayAsync(float spawnDelay, CancellationToken token)
        {
            float timer = 0f;
            
            while (timer < spawnDelay)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                
                if (!_pauseService.IsPaused)
                    timer += Time.deltaTime;
            }
        }
        
        private async UniTask InitEnemyAsync(Transform target)
        {
            EnemyContainer enemy = await _factory.CreateEnemyAsync(GetSpawnPosition(target));
            enemy.EnemyDeath.OnDied += OnEnemyDeath;
            _spawnedEnemies.Add(enemy);
            return;

            void OnEnemyDeath()
            {
                enemy.EnemyDeath.OnDied -= OnEnemyDeath;
                _spawnedEnemies.Remove(enemy);
            }
        }

        private Vector3 GetSpawnPosition(Transform target)
        {
            Vector2 spawnDirection = Random.insideUnitCircle * _config.SpawnDistance;
            Vector3 offset = new Vector3(spawnDirection.x, 0, spawnDirection.y);
            Vector3 spawnPosition = target.position + offset;
            return spawnPosition;
        }
    }
}
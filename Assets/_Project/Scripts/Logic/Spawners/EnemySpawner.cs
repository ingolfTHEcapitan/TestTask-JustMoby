using System;
using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Enemy;
using _Project.Scripts.Infrastructure.Services.Factory;
using _Project.Scripts.Infrastructure.Services.GamePause;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Logic.Spawners
{
    [UsedImplicitly]
    public class EnemySpawner
    {
        private readonly IGamePauseService _pauseService;
        private readonly IGameFactory _factory;
        private readonly List<EnemyDeath> _spawnedEnemies = new List<EnemyDeath>();
        private readonly EnemySpawnerConfig _config;

        public EnemySpawner(EnemySpawnerConfig config, IGamePauseService pauseService, IGameFactory factory)
        {
            _pauseService = pauseService;
            _factory = factory;
            _config = config;
        }
        
        public async UniTaskVoid SpawnAround(Transform target)
        {
            while (true)
            {
                await UniTask.WaitWhile(() => _pauseService.IsPaused);
                
                if (_spawnedEnemies.Count < _config.EnemiesAtTime)
                {
                    InitEnemy(target);
                    await UniTask.Delay(TimeSpan.FromSeconds(_config.SpawnDelay));
                }
                
                await UniTask.WaitUntil(() => _spawnedEnemies.Count < _config.EnemiesAtTime);
            }
        }

        private void InitEnemy(Transform target)
        {
            GameObject enemy = _factory.CreateEnemy(_config, GetSpawnPosition(target));
            EnemyDeath enemyDeath = enemy.GetComponent<EnemyDeath>();
            enemyDeath.OnDied += OnEnemyDeath;
            _spawnedEnemies.Add(enemyDeath);
        }

        private Vector3 GetSpawnPosition(Transform target)
        {
            Vector2 spawnDirection = Random.insideUnitCircle * _config.SpawnDistance;
            Vector3 offset = new Vector3(spawnDirection.x, 0, spawnDirection.y);
            Vector3 SpawnPosition = target.position + offset;
            return SpawnPosition;
        }

        private void OnEnemyDeath(EnemyDeath enemyDeath)
        {
            enemyDeath.OnDied -= OnEnemyDeath;
            _spawnedEnemies.Remove(enemyDeath);
        }
    }
}
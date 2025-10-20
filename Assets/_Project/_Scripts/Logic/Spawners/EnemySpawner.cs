using System.Collections;
using System.Collections.Generic;
using _Project._Scripts.Configs;
using _Project._Scripts.Enemy;
using _Project._Scripts.Infrastructure.Services.ConfigsManagement;
using _Project._Scripts.Infrastructure.Services.Factory;
using _Project._Scripts.Infrastructure.Services.GamePause;
using UnityEngine;

namespace _Project._Scripts.Logic.Spawners
{
    public class EnemySpawner
    {
        private readonly IGamePauseService _pauseService;
        private readonly IGameFactory _factory;
        private readonly List<EnemyDeath> _spawnedEnemies = new List<EnemyDeath>();
        private readonly EnemySpawnerConfig _config;

        public EnemySpawner(IConfigsProvider configs, IGamePauseService pauseService, IGameFactory factory)
        {
            _pauseService = pauseService;
            _factory = factory;
            _config = configs.EnemySpawner;
        }
        
        public IEnumerator SpawnAround(Transform target)
        {
            while (true)
            {
                yield return new WaitWhile(() => _pauseService.IsPaused);
                
                if (_spawnedEnemies.Count < _config.EnemiesAtTime)
                {
                    InitEnemy(target);
                    yield return new WaitForSeconds(_config.SpawnDelay);
                }
                
                yield return new WaitUntil(() => _spawnedEnemies.Count < _config.EnemiesAtTime);
            }
        }

        private void InitEnemy(Transform target)
        {
            GameObject enemy = _factory.CreateEnemy(_config, at: GetSpawnPosition(target));
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
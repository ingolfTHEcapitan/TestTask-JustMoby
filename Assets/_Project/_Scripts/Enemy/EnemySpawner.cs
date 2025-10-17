using System.Collections;
using System.Collections.Generic;
using _Project._Scripts.Configs;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.StatSystem;
using _Project._Scripts.UI;
using UnityEngine;

namespace _Project._Scripts.Enemy
{
    public class EnemySpawner
    {
        private readonly EnemySpawnerConfig _config;
        private readonly PlayerStatsSystem _playerStatsSystem;
        private readonly IGamePauseService _pauseService;
        private readonly Transform _enemyParent;
        private readonly List<EnemyDeath> _spawnedEnemies = new List<EnemyDeath>();

        public EnemySpawner(EnemySpawnerConfig config, PlayerStatsSystem playerStatsSystem,
            IGamePauseService pauseService, Transform enemyParent)
        {
            _config = config;
            _playerStatsSystem = playerStatsSystem;
            _pauseService = pauseService;
            _enemyParent = enemyParent;
        }

        public IEnumerator SpawnAround(Transform target)
        {
            while (true)
            {
                yield return new WaitWhile(() => _pauseService.IsPaused);
                
                if (_spawnedEnemies.Count < _config.EnemiesAtTime)
                {
                    CreateEnemy(target);
                    
                    yield return new WaitForSeconds(_config.SpawnDelay);
                }
                
                yield return new WaitUntil(() => _spawnedEnemies.Count < _config.EnemiesAtTime);
            }
        }

        private void CreateEnemy(Transform target)
        {
            GameObject enemy = Object.Instantiate(_config.Prefab, GetSpawnPosition(target), Quaternion.identity, _enemyParent);
            
            EnemyAgent enemyAgent = enemy.GetComponent<EnemyAgent>();
            enemyAgent.Construct(_pauseService);

            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            
            HealthBarView playerHealthBarView = enemy.GetComponentInChildren<HealthBarView>();
            playerHealthBarView.Construct(enemyHealth);
            playerHealthBarView.Initialize();
            
            EnemyDeath enemyDeath = enemy.GetComponent<EnemyDeath>();
            _spawnedEnemies.Add(enemyDeath);
            enemyDeath.OnDied += OnEnemyDeath;
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
            _playerStatsSystem.AddUpgradePoint();
        }
    }
}
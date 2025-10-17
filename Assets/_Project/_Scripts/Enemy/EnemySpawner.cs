using System.Collections;
using System.Collections.Generic;
using _Project._Scripts.Configs;
using _Project._Scripts.Player.StatSystem;
using _Project._Scripts.Services.GamePause;
using UnityEngine;

namespace _Project._Scripts.Enemy
{
    public class EnemySpawner
    {
        private readonly EnemySpawnerConfig _config;
        private readonly PlayerStatsSystem _playerStatsSystem;
        private readonly List<GameObject> _spawnedEnemies = new List<GameObject>();
        private IGamePauseService _pauseService;

        public EnemySpawner(EnemySpawnerConfig config, PlayerStatsSystem playerStatsSystem, IGamePauseService pauseService)
        {
            _config = config;
            _playerStatsSystem = playerStatsSystem;
            _pauseService = pauseService;
        }

        public IEnumerator SpawnAround(Transform target)
        {
            while (true)
            {
                if (_pauseService.IsPaused)
                {
                    yield return null;
                }
                else
                {
                    for (int i = 0; i < _config.EnemiesAtTime; i++)
                    {
                        if (_spawnedEnemies.Count < _config.EnemiesAtTime)
                        {
                            Vector2 spawnDirection = Random.insideUnitCircle * _config.SpawnDistance;
                            Vector3 offset = new Vector3(spawnDirection.x, 0, spawnDirection.y);
                            Vector3 SpawnPosition = target.position + offset;
                
                            GameObject enemy = Object.Instantiate(_config.Prefab, SpawnPosition, Quaternion.identity);
                            _spawnedEnemies.Add(enemy);
                            
                            EnemyDeath enemyDeath = enemy.GetComponent<EnemyDeath>();
                            enemyDeath.OnDied += OnEnemyDeath;

                            enemy.GetComponent<EnemyAgent>().Construct(_pauseService);
                            continue;

                            void OnEnemyDeath()
                            {
                                _spawnedEnemies.Remove(enemy);
                                _playerStatsSystem.AddUpgradePoint();
                                enemyDeath.OnDied -= OnEnemyDeath;
                            }
                        }
                    
                        yield return new WaitUntil(()=>_spawnedEnemies.Count == _config.EnemiesAtTime);
                    }
                }
            }
        }
    }
}
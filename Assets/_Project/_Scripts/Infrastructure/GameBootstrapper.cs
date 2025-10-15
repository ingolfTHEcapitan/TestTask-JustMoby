using _Project._Scripts.Configs;
using _Project._Scripts.Enemy;
using UnityEngine;

namespace _Project._Scripts.Infrastructure
{
    public class GameBootstrapper: MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;

        private void Awake()
        {
            EnemySpawner enemySpawner = new EnemySpawner(_enemySpawnerConfig);
            StartCoroutine(enemySpawner.SpawnAround(_playerPrefab.transform));
        }
    }
}
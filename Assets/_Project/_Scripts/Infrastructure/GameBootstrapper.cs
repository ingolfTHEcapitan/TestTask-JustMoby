using _Project._Scripts.Configs;
using _Project._Scripts.Enemy;
using _Project._Scripts.Player;
using _Project._Scripts.Player.StatSystem;
using _Project._Scripts.SaveLoad;
using UnityEngine;

namespace _Project._Scripts.Infrastructure
{
    public class GameBootstrapper: MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;

        private void Awake()
        {
            SaveLoadService saveLoadService = new SaveLoadService();
            
            PlayerStatsSystem playerStatsSystem = _playerPrefab.GetComponent<PlayerStatsSystem>();
            playerStatsSystem.Construct(saveLoadService);
            playerStatsSystem.Initialize();

            PlayerHealth playerHealth = _playerPrefab.GetComponent<PlayerHealth>();
            playerHealth.Construct(playerStatsSystem);
            playerHealth.Initialize();
            
            PlayerMovement playerMovement = _playerPrefab.GetComponent<PlayerMovement>();
            playerMovement.Construct(playerStatsSystem);
            
            Weapon.Weapon weapon = _playerPrefab.GetComponentInChildren<Weapon.Weapon>();
            weapon.Construct(playerStatsSystem);
            
            EnemySpawner enemySpawner = new EnemySpawner(_enemySpawnerConfig);
            StartCoroutine(enemySpawner.SpawnAround(_playerPrefab.transform));
        }
    }
}
using _Project._Scripts.Configs;
using _Project._Scripts.Enemy;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.SaveLoad;
using _Project._Scripts.Player;
using _Project._Scripts.StatSystem;
using _Project._Scripts.UI;
using _Project._Scripts.Utility;
using UnityEngine;

namespace _Project._Scripts.Infrastructure
{
    public class GameBootstrapper: MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;
        [SerializeField] private UpgradeStatsWindow _upgradeStatsWindow;
        
        private void Awake()
        {
            CursorController.SetCursorVisible(visible: false);
            
            IGamePauseService pauseService = new GamePauseService();
            ISaveLoadService saveLoadService = new SaveLoadService();
            
            PlayerStatsSystem playerStatsSystem = _playerPrefab.GetComponent<PlayerStatsSystem>();
            playerStatsSystem.Construct(saveLoadService);
            playerStatsSystem.Initialize();

            _upgradeStatsWindow.Construct(playerStatsSystem, pauseService);
            _upgradeStatsWindow.Initialize();
            
            PlayerHealth playerHealth = _playerPrefab.GetComponent<PlayerHealth>();
            playerHealth.Construct(playerStatsSystem);
            playerHealth.Initialize();
            
            PlayerCameraLook playerCameraLook = _playerPrefab.GetComponent<PlayerCameraLook>();
            playerCameraLook.Construct(pauseService);
            
            PlayerMovement playerMovement = _playerPrefab.GetComponent<PlayerMovement>();
            playerMovement.Construct(playerStatsSystem, pauseService);
            
            Weapon.Weapon weapon = _playerPrefab.GetComponentInChildren<Weapon.Weapon>();
            weapon.Construct(playerStatsSystem, pauseService);
            
            EnemySpawner enemySpawner = new EnemySpawner(_enemySpawnerConfig, playerStatsSystem, pauseService);
            StartCoroutine(enemySpawner.SpawnAround(_playerPrefab.transform));
        }
    }
}
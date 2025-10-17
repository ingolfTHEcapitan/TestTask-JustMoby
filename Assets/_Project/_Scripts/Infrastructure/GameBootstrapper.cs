using _Project._Scripts.Configs;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.PlayerInput;
using _Project._Scripts.Infrastructure.Services.SaveLoad;
using _Project._Scripts.Logic;
using _Project._Scripts.Logic.StatSystem;
using _Project._Scripts.Logic.Weapon;
using _Project._Scripts.Player;
using _Project._Scripts.UI.Elements;
using _Project._Scripts.UI.Windows.UpgradeStats;
using UnityEngine;

namespace _Project._Scripts.Infrastructure
{
    public class GameBootstrapper: MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _hudPrefab;
        [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;
        [SerializeField] private UpgradeStatsWindow _upgradeStatsWindow;
        [SerializeField] private Transform _dynamicObjectsRoot;
        
        private void Awake()
        {
            CursorController.SetCursorVisible(visible: false);

            IInputService inputService = new DesktopInputService();
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
            
            HealthBarView playerHealthBarView = _hudPrefab.GetComponentInChildren<HealthBarView>();
            playerHealthBarView.Construct(playerHealth);
            playerHealthBarView.Initialize();
            
            PlayerCameraLook playerCameraLook = _playerPrefab.GetComponent<PlayerCameraLook>();
            playerCameraLook.Construct(pauseService, inputService);
            
            PlayerMovement playerMovement = _playerPrefab.GetComponent<PlayerMovement>();
            playerMovement.Construct(playerStatsSystem, pauseService, inputService);
            
            Weapon weapon = _playerPrefab.GetComponentInChildren<Weapon>();
            weapon.Construct(playerStatsSystem, pauseService, inputService, _dynamicObjectsRoot);
            
            EnemySpawner enemySpawner = new EnemySpawner(_enemySpawnerConfig, playerStatsSystem, pauseService, _dynamicObjectsRoot);
            StartCoroutine(enemySpawner.SpawnAround(_playerPrefab.transform));
        }
    }
}
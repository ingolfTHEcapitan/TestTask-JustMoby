using _Project._Scripts.Configs;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.PlayerInput;
using _Project._Scripts.Infrastructure.Services.SaveLoad;
using _Project._Scripts.Logic;
using _Project._Scripts.Logic.PlayerStats;
using _Project._Scripts.Logic.Weapon;
using _Project._Scripts.Player;
using _Project._Scripts.UI.Elements;
using _Project._Scripts.UI.Windows.PlayerStats;
using UnityEngine;

namespace _Project._Scripts.Infrastructure
{
    public class GameBootstrapper: MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _hudPrefab;
        [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;
        [SerializeField] private PlayerStatsView playerStatsView;
        [SerializeField] private Transform _dynamicObjectsRoot;
        
        private void Awake()
        {
            CursorController.SetCursorVisible(visible: false);

            IInputService inputService = new DesktopInputService();
            IGamePauseService pauseService = new GamePauseService();
            ISaveLoadService saveLoadService = new SaveLoadService();
            
            PlayerStatsModel playerStatsModel = _playerPrefab.GetComponent<PlayerStatsModel>();
            playerStatsModel.Construct(saveLoadService);
            playerStatsModel.Initialize();

            PlayerStatsPresenter playerStatsPresenter = new PlayerStatsPresenter(playerStatsView, playerStatsModel, pauseService);
            playerStatsView.Construct(playerStatsPresenter);
            playerStatsPresenter.Initialize(playerStatsModel.GetStats());
            
            PlayerHealth playerHealth = _playerPrefab.GetComponent<PlayerHealth>();
            playerHealth.Construct(playerStatsModel);  
            playerHealth.Initialize();
            
            HealthBarView playerHealthBarView = _hudPrefab.GetComponentInChildren<HealthBarView>();
            playerHealthBarView.Construct(playerHealth);
            playerHealthBarView.Initialize();
            
            PlayerCameraLook playerCameraLook = _playerPrefab.GetComponent<PlayerCameraLook>();
            playerCameraLook.Construct(pauseService, inputService);
            
            PlayerMovement playerMovement = _playerPrefab.GetComponent<PlayerMovement>();
            playerMovement.Construct(playerStatsModel, pauseService, inputService);
            
            Weapon weapon = _playerPrefab.GetComponentInChildren<Weapon>();
            weapon.Construct(playerStatsModel, pauseService, inputService, _dynamicObjectsRoot);
            
            EnemySpawner enemySpawner = new EnemySpawner(_enemySpawnerConfig, playerStatsModel, pauseService, _dynamicObjectsRoot);
            StartCoroutine(enemySpawner.SpawnAround(_playerPrefab.transform));
        }
    }
}
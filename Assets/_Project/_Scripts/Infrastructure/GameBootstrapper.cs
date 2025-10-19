using System;
using _Project._Scripts.Configs;
using _Project._Scripts.Infrastructure.Services.AssetManagement;
using _Project._Scripts.Infrastructure.Services.ConfigsManagement;
using _Project._Scripts.Infrastructure.Services.Factory;
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
using UnityEngine.Serialization;

namespace _Project._Scripts.Infrastructure
{
    public class GameBootstrapper: MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _hudPrefab;
        [SerializeField] private PlayerStatsView playerStatsView;
        [SerializeField] private Transform _dynamicObjectsParent;
        
        private PlayerStatsModel _playerStatsModel;
        private PlayerStatsPresenter _playerStatsPresenter;

        private void Awake()
        {
            CursorController.SetCursorVisible(visible: false);

            IInputService inputService = new DesktopInputService();
            IGamePauseService pauseService = new GamePauseService();
            ISaveLoadService saveLoadService = new SaveLoadService();
            IAssetProvider assets = new AssetProvider();
            IConfigsProvider configs = InitializeConfigsProvider();
            
            _playerStatsModel = new PlayerStatsModel();
            _playerStatsModel.Construct(saveLoadService, configs);
            _playerStatsModel.Initialize();
            
            IGameFactory factory = new GameFactory(assets, pauseService, _playerStatsModel, _dynamicObjectsParent);

            _playerStatsPresenter = new PlayerStatsPresenter(playerStatsView, _playerStatsModel, pauseService);
            playerStatsView.Construct(_playerStatsPresenter);
            _playerStatsPresenter.Initialize(_playerStatsModel.GetStats());
            
            PlayerHealth playerHealth = _playerPrefab.GetComponent<PlayerHealth>();
            playerHealth.Construct(_playerStatsModel);  
            playerHealth.Initialize();
            
            HealthBarView playerHealthBarView = _hudPrefab.GetComponentInChildren<HealthBarView>();
            playerHealthBarView.Construct(playerHealth);
            playerHealthBarView.Initialize();
            
            PlayerCameraLook playerCameraLook = _playerPrefab.GetComponent<PlayerCameraLook>();
            playerCameraLook.Construct(pauseService, inputService);
            
            PlayerMovement playerMovement = _playerPrefab.GetComponent<PlayerMovement>();
            playerMovement.Construct(_playerStatsModel, pauseService, inputService);
            
            Weapon weapon = _playerPrefab.GetComponentInChildren<Weapon>();
            weapon.Construct(pauseService, inputService, factory);
            
            EnemySpawner enemySpawner = new EnemySpawner(configs, pauseService, factory);
            StartCoroutine(enemySpawner.SpawnAround(_playerPrefab.transform));
        }

        private void OnDestroy()
        {
            _playerStatsModel.Dispose();
            _playerStatsPresenter.Dispose();
        }

        private static ConfigsProvider InitializeConfigsProvider()
        {
            ConfigsProvider configsProvider = new ConfigsProvider();
            configsProvider.LoadEnemySpawner();
            configsProvider.LoadPlayerStats();
            return configsProvider;
        }
    }
}
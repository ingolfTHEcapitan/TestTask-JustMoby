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

namespace _Project._Scripts.Infrastructure
{
    public class GameBootstrapper: MonoBehaviour
    {
        [SerializeField] private Transform _dynamicObjectsParent;
        [SerializeField] private Transform _UIParent;
        [SerializeField] private Transform _gameParent;
        
        private PlayerStatsModel _playerStatsModel;
        private PlayerStatsPresenter _playerStatsPresenter;

        private void Awake()
        {
            CursorController.SetCursorVisible(visible: false);

            IInputService inputService = new DesktopInputService();
            IGamePauseService pauseService = new GamePauseService();
            ISaveLoadService saveLoadService = new SaveLoadService();
            IAssetProvider assets = new AssetProvider();
            IConfigsProvider configsProvider = InitializeConfigsProvider();
            
            _playerStatsModel = new PlayerStatsModel();
            _playerStatsModel.Construct(saveLoadService, configsProvider);
            _playerStatsModel.Initialize();
            
            IGameFactory factory = new GameFactory(assets, pauseService, inputService, _playerStatsModel, _dynamicObjectsParent);

            GameObject popUpLayer = factory.CreatePopUpLayer(_UIParent);
            GameObject hud = factory.CreateHud(_UIParent);
            OpenWindowButton openButton = hud.GetComponentInChildren<OpenWindowButton>();
            
            PlayerStatsView playerStatsView = popUpLayer.GetComponent<PlayerStatsView>();
            _playerStatsPresenter = new PlayerStatsPresenter(playerStatsView, _playerStatsModel, pauseService);
            playerStatsView.Construct(_playerStatsPresenter, openButton);
            _playerStatsPresenter.Initialize(_playerStatsModel.GetStats());

            PlayerSpawner playerSpawner = new PlayerSpawner(factory, configsProvider);
            GameObject Player = playerSpawner.Spawn(_gameParent);
            PlayerHealth playerHealth = Player.GetComponent<PlayerHealth>();

            HealthBarView playerHealthBarView = hud.GetComponentInChildren<HealthBarView>();
            playerHealthBarView.Construct(playerHealth);
            playerHealthBarView.Initialize();

            Weapon weapon = Player.GetComponentInChildren<Weapon>();
            weapon.Construct(pauseService, inputService, factory);
            
            EnemySpawner enemySpawner = new EnemySpawner(configsProvider, pauseService, factory);
            StartCoroutine(enemySpawner.SpawnAround(Player.transform));
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
            configsProvider.LoadPlayerSpawner();
            return configsProvider;
        }
    }
}
using _Project._Scripts.Infrastructure.Services.AssetManagement;
using _Project._Scripts.Infrastructure.Services.ConfigsManagement;
using _Project._Scripts.Infrastructure.Services.Factory;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.PlayerInput;
using _Project._Scripts.Infrastructure.Services.SaveLoad;
using _Project._Scripts.Logic;
using _Project._Scripts.Logic.PlayerStats;
using _Project._Scripts.Logic.Spawners;
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
        [SerializeField] private Transform _enemySpawnPoint;
        
        private PlayerStatsModel _playerStatsModel;
        private PlayerStatsPresenter _playerStatsPresenter;

        private void Awake()
        {
            CursorController.SetCursorVisible(visible: false);

            IInputService inputService = new DesktopInputService();
            IGamePauseService pauseService = new GamePauseService();
            ISaveLoadService saveLoadService = new SaveLoadService();
            IAssetProvider assets = new AssetProvider();
            IConfigsProvider configs = InitConfigsProvider();

            _playerStatsModel = InitPlayerStatsModel(saveLoadService, configs);
            IGameFactory factory = new GameFactory(assets, pauseService, inputService, 
                _playerStatsModel, _dynamicObjectsParent, _enemySpawnPoint);

            GameObject Player = InitPlayer(factory, configs);
            GameObject hud = InitHud(factory, Player);
            GameObject popUpLayer = InitPopUpLayer(factory);
            
            InitWeapon(Player, pauseService, inputService, factory);
            InitPlayerStatsWindow(popUpLayer, hud, pauseService);
            InitEnemySpawner(configs, pauseService, factory, _enemySpawnPoint);
        }

        private void OnDestroy()
        {
            _playerStatsModel.Dispose();
            _playerStatsPresenter.Dispose();
        }

        private PlayerStatsModel InitPlayerStatsModel(ISaveLoadService saveLoadService, IConfigsProvider configsProvider)
        {
            PlayerStatsModel playerStatsModel = new PlayerStatsModel();
            playerStatsModel.Construct(saveLoadService, configsProvider);
            playerStatsModel.Initialize();
            return playerStatsModel;
        }

        private GameObject InitPlayer(IGameFactory factory, IConfigsProvider configs)
        {
            PlayerSpawner playerSpawner = new PlayerSpawner(factory, configs);
            return playerSpawner.Spawn(_gameParent);
        }

        private GameObject InitHud(IGameFactory factory, GameObject Player)
        {
            GameObject hud = factory.CreateHud(_UIParent);
            
            HealthBarView playerHealthBarView = hud.GetComponentInChildren<HealthBarView>();
            playerHealthBarView.Construct(Player.GetComponent<PlayerHealth>());
            playerHealthBarView.Initialize();
            return hud;
        }

        private GameObject InitPopUpLayer(IGameFactory factory) => 
            factory.CreatePopUpLayer(_UIParent);

        private void InitWeapon(GameObject Player, IGamePauseService pauseService, 
            IInputService inputService, IGameFactory factory)
        {
            Weapon weapon = Player.GetComponentInChildren<Weapon>();
            weapon.Construct(pauseService, inputService, factory);
        }

        private void InitPlayerStatsWindow(GameObject popUpLayer, GameObject hud, IGamePauseService pauseService)
        {
            PlayerStatsView playerStatsView = popUpLayer.GetComponent<PlayerStatsView>();
            OpenWindowButton openButton = hud.GetComponentInChildren<OpenWindowButton>();
            
            _playerStatsPresenter = new PlayerStatsPresenter(playerStatsView, _playerStatsModel, pauseService);
            playerStatsView.Construct(_playerStatsPresenter, openButton);
            _playerStatsPresenter.Initialize(_playerStatsModel.GetStats());
        }

        private void InitEnemySpawner(IConfigsProvider configs, IGamePauseService pauseService, 
            IGameFactory factory, Transform target)
        {
            EnemySpawner enemySpawner = new EnemySpawner(configs, pauseService, factory);
            StartCoroutine(enemySpawner.SpawnAround(target));
        }

        private ConfigsProvider InitConfigsProvider()
        {
            ConfigsProvider configsProvider = new ConfigsProvider();
            configsProvider.Initialize();
            return configsProvider;
        }
    }
}
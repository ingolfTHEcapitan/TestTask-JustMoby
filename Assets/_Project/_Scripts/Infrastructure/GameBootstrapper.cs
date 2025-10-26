using System.Collections.Generic;
using _Project._Scripts.Configs;
using _Project._Scripts.Configs.Weapon;
using _Project._Scripts.Infrastructure.Services.Factory;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.PlayerInput;
using _Project._Scripts.Infrastructure.Services.SaveLoad;
using _Project._Scripts.Logic;
using _Project._Scripts.Logic.PlayerStats;
using _Project._Scripts.Logic.Spawners;
using _Project._Scripts.Logic.Weapon;
using _Project._Scripts.UI.Elements;
using _Project._Scripts.UI.Windows.PlayerStats;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Infrastructure
{
    public class GameBootstrapper: MonoBehaviour
    {
        [Header("Transforms")]
        [SerializeField] private Transform _dynamicObjectsParent;
        [SerializeField] private Transform _UIParent;
        [SerializeField] private Transform _gameParent;
        [SerializeField] private Transform _enemySpawnPoint;
        [Header("Configs")]
        [SerializeField] private List<PlayerStatConfig> _playerStatConfigs;
        [SerializeField] private PlayerSpawnerConfig _playerSpawnerConfig;
        [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;
        [SerializeField] private WeaponConfig _weaponConfig;
        [Header("Prefabs")]
        [SerializeField] private GameObject _hudPrefab;
        [SerializeField] private GameObject _popUpLayerPrefab;
        
        private PlayerStatsModel _playerStatsModel;
        private PlayerStatsPresenter _playerStatsPresenter;
        private HealthCalculator _healthCalculator;
        private Health _playerHealth;

        private void Awake()
        {
            CursorController.SetCursorVisible(visible: false);

            IInputService inputService = new DesktopInputService();
            IGamePauseService pauseService = new GamePauseService();
            ISaveLoadService saveLoadService = new SaveLoadService();

            _playerStatsModel = InitPlayerStatsModel(saveLoadService, _playerStatConfigs);
            _healthCalculator = new HealthCalculator(_playerStatsModel);
            IGameFactory factory = new GameFactory(pauseService, inputService, _healthCalculator, _playerStatsModel,
                _dynamicObjectsParent, _hudPrefab, _popUpLayerPrefab);

            GameObject Player = InitPlayer(factory, _playerSpawnerConfig);
            GameObject hud = InitHud(factory, Player);
            GameObject popUpLayer = InitPopUpLayer(factory);
            
            PlayerStatsView playerStatsView = InitPlayerStatsView(popUpLayer, hud);
            _playerStatsPresenter = InitPlayerStatsPresenter(playerStatsView, _playerStatsModel, pauseService);
            
            InitWeapon(Player, pauseService, inputService, factory);
            InitEnemySpawner(_enemySpawnerConfig, pauseService, factory, _enemySpawnPoint);
        }

        private void OnDestroy()
        {
            _playerStatsModel.Dispose();
            _playerStatsPresenter.Dispose();
            _playerStatsModel.OnStatsChanged -= UpdatePlayerMaxHealth;
        }

        private PlayerStatsModel InitPlayerStatsModel(ISaveLoadService saveLoadService, List<PlayerStatConfig> configs)
        {
            PlayerStatsModel playerStatsModel = new PlayerStatsModel();
            playerStatsModel.Construct(saveLoadService);
            playerStatsModel.Initialize(configs);
            return playerStatsModel;
        }

        private GameObject InitPlayer(IGameFactory factory, PlayerSpawnerConfig config)
        {
            PlayerSpawner playerSpawner = new PlayerSpawner(factory, config);
            GameObject player = playerSpawner.Spawn(_gameParent);
            
            _playerHealth = player.GetComponent<Health>();
            _playerStatsModel.OnStatsChanged += UpdatePlayerMaxHealth;
            return player;
        }

        private void UpdatePlayerMaxHealth()
        {
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            _playerHealth.Initialize(maxHealth);
            _playerHealth.InvokeOnHealthChanged();
        }

        private GameObject InitHud(IGameFactory factory, GameObject Player)
        {
            GameObject hud = factory.CreateHud(_UIParent);
            
            HealthBarView playerHealthBarView = hud.GetComponentInChildren<HealthBarView>();
            playerHealthBarView.Construct(Player.GetComponent<Health>());
            playerHealthBarView.Initialize();
            return hud;
        }

        private GameObject InitPopUpLayer(IGameFactory factory) => 
            factory.CreatePopUpLayer(_UIParent);

        private void InitWeapon(GameObject Player, IGamePauseService pauseService, 
            IInputService inputService, IGameFactory factory)
        {
            Weapon weapon = Player.GetComponentInChildren<Weapon>();
            Camera playerCamera = Player.GetComponentInChildren<Camera>();
            weapon.Construct(pauseService, inputService, factory);
            weapon.Initialize(_weaponConfig, playerCamera);
        }

        private PlayerStatsView InitPlayerStatsView(GameObject popUpLayer, GameObject hud)
        {
            Button openButton = hud.GetComponentInChildren<Button>();
            
            PlayerStatsView playerStatsView = popUpLayer.GetComponent<PlayerStatsView>();
            playerStatsView.Construct(openButton);
            playerStatsView.Initialize();
            return playerStatsView;
        }

        private PlayerStatsPresenter InitPlayerStatsPresenter(PlayerStatsView view, PlayerStatsModel model , IGamePauseService pauseService)
        {
            PlayerStatsPresenter playerStatsPresenter = new PlayerStatsPresenter(view, model, pauseService);
            playerStatsPresenter.Initialize();
            return playerStatsPresenter;
        }

        private void InitEnemySpawner(EnemySpawnerConfig config, IGamePauseService pauseService, 
            IGameFactory factory, Transform target)
        {
            EnemySpawner enemySpawner = new EnemySpawner(config, pauseService, factory);
            StartCoroutine(enemySpawner.SpawnAround(target));
        }
    }
}
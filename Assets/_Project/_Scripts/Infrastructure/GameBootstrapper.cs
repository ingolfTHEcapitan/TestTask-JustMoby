using _Project._Scripts.Infrastructure.Services.Factory;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.HealthCalculator;
using _Project._Scripts.Infrastructure.Services.SaveLoad;
using _Project._Scripts.Logic;
using _Project._Scripts.Logic.PlayerStats;
using _Project._Scripts.Logic.Spawners;
using _Project._Scripts.Logic.Weapon;
using _Project._Scripts.UI.Elements;
using _Project._Scripts.UI.Windows.PlayerStats;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project._Scripts.Infrastructure
{
    public class GameBootstrapper: MonoBehaviour
    {
        [SerializeField] private Transform _enemySpawnPoint;
        [Header("Prefabs")]
        [SerializeField] private GameObject _hudPrefab;
        [SerializeField] private GameObject _popUpLayerPrefab;

        private IGamePauseService _pauseService;
        private IGameFactory _factory;
        private IHealthCalculatorService _healthCalculator;
        private PlayerStatsModel _playerStatsModel;
        private PlayerStatsPresenter _playerStatsPresenter;
        private PlayerSpawner _playerSpawner;
        private EnemySpawner _enemySpawner;
        private Health _playerHealth;
        
        [Inject]
        public void Construct(IGamePauseService pauseService, ISaveLoadService saveLoadService, 
            IGameFactory factory, IHealthCalculatorService healthCalculator, PlayerStatsModel playerStatsModel, 
            PlayerSpawner playerSpawner, EnemySpawner enemySpawner)
        {
            _pauseService = pauseService;
            _factory = factory;
            _healthCalculator = healthCalculator;
            _playerStatsModel = playerStatsModel;
            _playerSpawner = playerSpawner;
            _enemySpawner = enemySpawner;
        }
        
        private void Awake()
        {
            CursorController.SetCursorVisible(visible: false);
            
            GameObject hud = _factory.CreateHud(_hudPrefab);
            GameObject popUpLayer = _factory.CreatePopUpLayer(_popUpLayerPrefab);
            
            _playerStatsModel.Initialize();
            PlayerStatsView playerStatsView = InitPlayerStatsView(popUpLayer, hud);
            _playerStatsPresenter = InitPlayerStatsPresenter(playerStatsView, _playerStatsModel, _pauseService);
            
            GameObject Player = InitPlayer(_playerSpawner);
            InitPlayerHealthBarView(hud);
            InitWeapon(Player);
            
            InitEnemySpawner(_enemySpawner, _enemySpawnPoint);
        }

        private void OnDestroy()
        {
            _playerStatsModel.Dispose();
            _playerStatsPresenter.Dispose();
            _playerStatsModel.OnStatsChanged -= UpdatePlayerMaxHealth;
        }
        
        private GameObject InitPlayer(PlayerSpawner playerSpawner)
        {
            GameObject player = playerSpawner.Spawn();
            
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

        private void InitPlayerHealthBarView(GameObject hud)
        {
            HealthBarView playerHealthBarView = hud.GetComponentInChildren<HealthBarView>();
            playerHealthBarView.Construct(_playerHealth);
            playerHealthBarView.Initialize();
        }
        
        private void InitWeapon(GameObject Player)
        {
            Weapon weapon = Player.GetComponentInChildren<Weapon>();
            Camera playerCamera = Player.GetComponentInChildren<Camera>();
            weapon.Initialize(playerCamera);
        }

        private void InitEnemySpawner(EnemySpawner enemySpawner, Transform target) => 
            StartCoroutine(enemySpawner.SpawnAround(target));

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
    }
}
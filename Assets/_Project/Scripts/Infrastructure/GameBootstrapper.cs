using System;
using _Project.Scripts.Infrastructure.Services.Factory;
using _Project.Scripts.Infrastructure.Services.GamePause;
using _Project.Scripts.Infrastructure.Services.HealthCalculator;
using _Project.Scripts.Logic;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Logic.Weapon;
using _Project.Scripts.UI.Elements;
using _Project.Scripts.UI.Windows.PlayerStats;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Infrastructure
{
    public class GameBootstrapper: IInitializable, IDisposable
    {
        private readonly IGamePauseService _pauseService;
        private readonly IGameFactory _factory;
        private readonly IHealthCalculatorService _healthCalculator;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly PlayerSpawner _playerSpawner;
        private readonly EnemySpawner _enemySpawner;
        private readonly GameObject _hudLayerPrefab;
        private readonly GameObject _popUpLayerPrefab;
        private readonly Transform _enemySpawnPoint;
        private PlayerStatsPresenter _playerStatsPresenter;
        private Health _playerHealth;

        public GameBootstrapper(IGamePauseService pauseService, IGameFactory factory, IHealthCalculatorService healthCalculator, 
            PlayerStatsModel playerStatsModel, PlayerSpawner playerSpawner, EnemySpawner enemySpawner, 
            GameObject hudLayerPrefab, GameObject popUpLayerPrefab, Transform enemySpawnPoint)
        {
            _pauseService = pauseService;
            _factory = factory;
            _healthCalculator = healthCalculator;
            _playerStatsModel = playerStatsModel;
            _playerSpawner = playerSpawner;
            _enemySpawner = enemySpawner;
            _hudLayerPrefab = hudLayerPrefab;
            _popUpLayerPrefab = popUpLayerPrefab;
            _enemySpawnPoint = enemySpawnPoint;
        }

        public void Initialize()
        {
            CursorController.SetCursorVisible(visible: false);
            
            GameObject hud = _factory.CreateHudLayer(_hudLayerPrefab);
            GameObject popUpLayer = _factory.CreatePopUpLayer(_popUpLayerPrefab);
            
            _playerStatsModel.Initialize();
            PlayerStatsView playerStatsView = InitPlayerStatsView(popUpLayer, hud);
            _playerStatsPresenter = InitPlayerStatsPresenter(playerStatsView, _playerStatsModel, _pauseService);
            
            _playerHealth = InitPlayer(_playerSpawner);
            InitPlayerHealthBarView(hud);
            InitWeapon(_playerHealth.gameObject);
            
            InitEnemySpawner(_enemySpawner, _enemySpawnPoint);
        }

        public void Dispose()
        {
            _playerStatsModel.Dispose();
            _playerStatsPresenter.Dispose();
            _playerStatsModel.OnStatsChanged -= UpdatePlayerMaxHealth;
        }

        private Health InitPlayer(PlayerSpawner playerSpawner)
        {
            Health playerHealth = playerSpawner.Spawn();
            _playerStatsModel.OnStatsChanged += UpdatePlayerMaxHealth;
            return playerHealth;
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
            enemySpawner.SpawnAround(target).Forget();

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
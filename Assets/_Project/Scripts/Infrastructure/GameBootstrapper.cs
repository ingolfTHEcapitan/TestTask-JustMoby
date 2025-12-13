using System;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Logic.Weapon;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Factory.UIFactory;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.UI.Elements;
using _Project.Scripts.UI.Windows.GameOver;
using _Project.Scripts.UI.Windows.PlayerStats;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Infrastructure
{
    public class GameBootstrapper : IInitializable, IDisposable
    {
        private readonly IGamePauseService _pauseService;
        private readonly IUIFactory _uiFactory;
        private readonly IAnalyticsService _analyticsService;
        private readonly IAssetProvider _assetProvider;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly PlayerSpawner _playerSpawner;
        private readonly EnemySpawner _enemySpawner;
        private readonly Transform _enemySpawnPoint;
        private PlayerStatsPresenter _playerStatsPresenter;

        public GameBootstrapper(IGamePauseService pauseService, IUIFactory uiFactory, IAssetProvider assetProvider, 
            PlayerStatsModel playerStatsModel, PlayerSpawner playerSpawner, EnemySpawner enemySpawner, 
            Transform enemySpawnPoint, IAnalyticsService analyticsService)
        {
            _pauseService = pauseService;
            _uiFactory = uiFactory;
            _assetProvider = assetProvider;
            _analyticsService = analyticsService;
            _playerStatsModel = playerStatsModel;
            _playerSpawner = playerSpawner;
            _enemySpawner = enemySpawner;
            _enemySpawnPoint = enemySpawnPoint;
        }

        public async void Initialize()
        {
            CursorController.SetCursorVisible(visible: false);
            
            GameObject hudLayer = await _uiFactory.CreateHudLayer();
            GameObject popUpLayer = await _uiFactory.CreatePopUpLayer();
            
            _playerStatsModel.Initialize();
            
            Health playerHealth = await InitPlayer(_playerSpawner);
            InitPlayerHealthBarView(hudLayer, playerHealth);
            InitWeapon(playerHealth);

            PlayerStatsView playerStatsView = InitPlayerStatsView(popUpLayer, hudLayer);
            _playerStatsPresenter = InitPlayerStatsPresenter(playerStatsView, _playerStatsModel, _pauseService, playerHealth);
            
            InitEnemySpawner(_enemySpawner, _enemySpawnPoint, playerHealth.transform);
            InitGameOverWindow(popUpLayer, playerHealth);
            
            _analyticsService.LogGameStart();
        }

        private void InitGameOverWindow(GameObject popUpLayer, Health player)
        {
            PlayerDeath playerDeath = player.GetComponent<PlayerDeath>();
            GameOverWindow gameOverWindow = popUpLayer.GetComponentInChildren<GameOverWindow>();
            gameOverWindow.Initialize(playerDeath);
        }

        public void Dispose()
        {
            _playerStatsPresenter.Dispose();
            _assetProvider.CleanUp();
        }

        private async Task<Health> InitPlayer(PlayerSpawner playerSpawner)
        {
            Health playerHealth = await playerSpawner.Spawn();
            return playerHealth;
        }
        
        private void InitPlayerHealthBarView(GameObject hud, Health playerHealth)
        {
            HealthBarView playerHealthBarView = hud.GetComponentInChildren<HealthBarView>();
            playerHealthBarView.Construct(playerHealth);
            playerHealthBarView.Initialize();
        }

        private void InitWeapon(Health player)
        {
            Weapon weapon = player.GetComponentInChildren<Weapon>();
            Camera playerCamera = player.GetComponentInChildren<Camera>();
            weapon.Initialize(playerCamera);
        }

        private void InitEnemySpawner(EnemySpawner enemySpawner, Transform target, Transform playerTransform) => 
            enemySpawner.SpawnAround(target, playerTransform);

        private PlayerStatsView InitPlayerStatsView(GameObject popUpLayer, GameObject hud)
        {
            Button openButton = hud.GetComponentInChildren<Button>();
            
            PlayerStatsView playerStatsView = popUpLayer.GetComponent<PlayerStatsView>();
            playerStatsView.Initialize(openButton);
            return playerStatsView;
        }

        private PlayerStatsPresenter InitPlayerStatsPresenter(PlayerStatsView view, PlayerStatsModel model,
            IGamePauseService pauseService, Health player)
        {
            PlayerDeath playerDeath = player.GetComponent<PlayerDeath>();
            
            PlayerStatsPresenter playerStatsPresenter = new PlayerStatsPresenter(view, model, pauseService, playerDeath);
            playerStatsPresenter.Initialize();
            return playerStatsPresenter;
        }
    }
}
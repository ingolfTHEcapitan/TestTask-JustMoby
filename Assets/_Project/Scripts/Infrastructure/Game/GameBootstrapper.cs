using System;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Player.PlayerStats;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using _Project.Scripts.Logic.Player.PlayerStats.UI;
using _Project.Scripts.Logic.Player.Weapon;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Effects;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using _Project.Scripts.UI.Windows.GameOver;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Infrastructure.Game
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
        private readonly Transform _uiParent;
        private readonly LoadingCurtainService _loadingCurtain;
        private PlayerStatsPresenter _playerStatsPresenter;
        private PlayerStatsData _playerStatsData;
        private EffectsService _effectsService;


        public GameBootstrapper(IGamePauseService pauseService, IUIFactory uiFactory, IAssetProvider assetProvider, 
            PlayerStatsModel playerStatsModel, PlayerStatsData playerStatsData, PlayerSpawner playerSpawner, 
            EnemySpawner enemySpawner, Transform enemySpawnPoint, IAnalyticsService analyticsService, Transform uiParent, 
            LoadingCurtainService loadingCurtain, EffectsService effectsService)
        {
            _pauseService = pauseService;
            _uiFactory = uiFactory;
            _assetProvider = assetProvider;
            _analyticsService = analyticsService;
            _playerStatsModel = playerStatsModel;
            _playerStatsData = playerStatsData;
            _playerSpawner = playerSpawner;
            _enemySpawner = enemySpawner;
            _enemySpawnPoint = enemySpawnPoint;
            _uiParent = uiParent;
            _loadingCurtain = loadingCurtain;
            _effectsService = effectsService;
        }

        public async void Initialize()
        {
            CursorController.SetCursorVisible(visible: false);

            await _effectsService.WarmUp();
            
            GameObject hudLayer = await _uiFactory.CreateHudLayer(_uiParent);
            GameObject popUpLayer = await _uiFactory.CreatePopUpLayer(_uiParent);
            
            await _playerStatsModel.Initialize();
            
            Health playerHealth = await InitPlayer(_playerSpawner);
            InitPlayerHealthBarView(hudLayer, playerHealth);
            InitWeapon(playerHealth);
            
            PlayerStatsView playerStatsView = InitPlayerStatsView(popUpLayer, hudLayer);
            _playerStatsPresenter = InitPlayerStatsPresenter(playerStatsView, _playerStatsModel, _pauseService, playerHealth);

            InitEnemySpawner(_enemySpawner, _enemySpawnPoint, playerHealth.transform);
            InitGameOverWindow(popUpLayer, playerHealth, _enemySpawner);

            _analyticsService.LogGameStart();
            _loadingCurtain.HideLoading();
        }

        public void Dispose()
        {
            _playerStatsPresenter.Dispose();
            _assetProvider.CleanUp();
        }

        private void InitGameOverWindow(GameObject popUpLayer, Health player, EnemySpawner enemySpawner)
        {
            PlayerDeath playerDeath = player.GetComponent<PlayerDeath>();
            GameOverWindow gameOverWindow = popUpLayer.GetComponentInChildren<GameOverWindow>();
            gameOverWindow.Initialize(playerDeath, enemySpawner);
        }

        private async UniTask<Health> InitPlayer(PlayerSpawner playerSpawner)
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
            
            PlayerStatsPresenter playerStatsPresenter = new PlayerStatsPresenter(view, model, _playerStatsData, pauseService, playerDeath);
            playerStatsPresenter.Initialize();
            return playerStatsPresenter;
        }
    }
}
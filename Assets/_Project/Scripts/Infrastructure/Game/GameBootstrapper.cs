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
using _Project.Scripts.Services.Sound;
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
        private readonly IEffectsService _effectsService;
        private readonly IAudioService _audioService;
        private readonly ILoadingCurtainService _loadingCurtain;
        
        private readonly PlayerSpawner _playerSpawner;
        private readonly EnemySpawner _enemySpawner;

        private readonly PlayerStatsData _playerStatsData;
        private readonly PlayerStatsModel _playerStatsModel;
        private PlayerStatsPresenter _playerStatsPresenter;

        private readonly Transform _enemySpawnPoint;
        private readonly Transform _uiParent;
        
        private readonly AudioSource _audioSource;
        private readonly AudioClip _dungeonMusic;


        public GameBootstrapper(IGamePauseService pauseService, IUIFactory uiFactory, IAnalyticsService analyticsService, 
            IAssetProvider assetProvider, IEffectsService effectsService, IAudioService audioService, 
            ILoadingCurtainService loadingCurtain, PlayerSpawner playerSpawner, EnemySpawner enemySpawner, 
            PlayerStatsData playerStatsData, PlayerStatsModel playerStatsModel, Transform enemySpawnPoint, Transform uiParent, 
            AudioSource audioSource, AudioClip dungeonMusic)
        {
            _pauseService = pauseService;
            _uiFactory = uiFactory;
            _analyticsService = analyticsService;
            _assetProvider = assetProvider;
            _effectsService = effectsService;
            _audioService = audioService;
            _loadingCurtain = loadingCurtain;
            _playerSpawner = playerSpawner;
            _enemySpawner = enemySpawner;
            _playerStatsData = playerStatsData;
            _playerStatsModel = playerStatsModel;
            _enemySpawnPoint = enemySpawnPoint;
            _uiParent = uiParent;
            _audioSource = audioSource;
            _dungeonMusic = dungeonMusic;
        }

        public async void Initialize()
        {
            CursorController.SetCursorVisible(visible: false);

            await _effectsService.WarmUp();
            await _playerStatsModel.Initialize();

            GameObject hudLayer = await _uiFactory.CreateHudLayer(_uiParent);
            GameObject popUpLayer = await _uiFactory.CreatePopUpLayer(_uiParent);
            
            Health playerHealth = await InitPlayer(_playerSpawner);
            InitPlayerHealthBarView(hudLayer, playerHealth);
            InitWeapon(playerHealth);
            
            PlayerStatsView playerStatsView = InitPlayerStatsView(popUpLayer, hudLayer);
            _playerStatsPresenter = await InitPlayerStatsPresenter(playerStatsView, _playerStatsModel, _pauseService, playerHealth);

            InitEnemySpawner(_enemySpawner, _enemySpawnPoint, playerHealth.transform);
            InitGameOverWindow(popUpLayer, playerHealth, _enemySpawner);

            _analyticsService.LogGameStart();
            _loadingCurtain.HideLoading();
            _audioService.Play(_dungeonMusic, _audioSource);
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

        private async UniTask<PlayerStatsPresenter> InitPlayerStatsPresenter(PlayerStatsView view, PlayerStatsModel model,
            IGamePauseService pauseService, Health player)
        {
            PlayerDeath playerDeath = player.GetComponent<PlayerDeath>();
            
            PlayerStatsPresenter playerStatsPresenter = new PlayerStatsPresenter(view, model, _playerStatsData, pauseService, playerDeath);
            await playerStatsPresenter.Initialize();
            return playerStatsPresenter;
        }
    }
}
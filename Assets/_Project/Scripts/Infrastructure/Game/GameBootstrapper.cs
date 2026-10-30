using System;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameBootstrapper : IInitializable
    {
        private readonly EffectsService _effectsService;
        private readonly PlayerSpawner _playerSpawner;
        private readonly EnemySpawner _enemySpawner;
        private readonly Transform _enemySpawnPoint;
        private readonly GameUIInitializer _uiInitializer;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly LoadingCurtainPresenter _loadingCurtainPresenter;
        private readonly IMusicService _audioService;
        private readonly AudioClip _gameplayMusic;
        private readonly IAnalyticsService _analyticsService;
        
        public GameBootstrapper(EffectsService effectsService, PlayerSpawner playerSpawner, EnemySpawner enemySpawner, 
            Transform enemySpawnPoint, GameUIInitializer uiInitializer, PlayerStatsModel playerStatsModel, LoadingCurtainPresenter loadingCurtainPresenter,
            IAnalyticsService analyticsService, IMusicService audioService, AudioSource audioSource, AudioClip gameplayMusic)
        {
            _effectsService = effectsService;
            _playerSpawner = playerSpawner;
            _enemySpawner = enemySpawner;
            _enemySpawnPoint = enemySpawnPoint;
            _uiInitializer = uiInitializer;
            _playerStatsModel = playerStatsModel;
            _loadingCurtainPresenter = loadingCurtainPresenter;
            _analyticsService = analyticsService;
            _audioService = audioService;
            _gameplayMusic = gameplayMusic;
        }

        public void Initialize() => 
            InitializeAsync().Forget();

        private async UniTaskVoid InitializeAsync()
        {
            try
            {
                await _effectsService.WarmUpAsync();
                _playerStatsModel.CreateStats();
            
                PlayerContainer player = await _playerSpawner.SpawnAsync();
            
                await _uiInitializer.InitUIAsync();
            
                _enemySpawner.SpawnAround( _enemySpawnPoint, player.transform);
            
                _analyticsService.LogGameStart();
                _audioService.PlayMusic(_gameplayMusic);
                _loadingCurtainPresenter.HideLoading();
            }
            catch (Exception e)
            {
                Debug.LogError($"[GAME BOOTSTRAPPER] init error: {e}");
                Debug.LogException(e);
            }
        }
    }
}
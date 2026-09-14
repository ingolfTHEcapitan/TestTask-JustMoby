using System;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.Ads;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.SceneLoader;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.UI.Windows.GameOver
{
    public class GameOverWindowModel: IDisposable
    {
        public event Action OnPlayerDied;
        public event Action OnRewardedAdLoaded;
        
        // M +
        private IGamePauseService _pauseService;
        private IAdsService _adsService;
        private ISceneLoaderService _sceneLoader;
        private PlayerDeath _playerDeath;
        private EnemySpawner _enemySpawner;
        
        // M +
        private bool _reviveInThisSession;
        
        public GameOverWindowModel(IGamePauseService pauseService, IAdsService adsService, 
            ISceneLoaderService sceneLoader, EnemySpawner enemySpawner)
        {
            _pauseService = pauseService;
            _adsService = adsService;
            _sceneLoader = sceneLoader;
            _enemySpawner = enemySpawner;
        }
        
        public void Construct(PlayerDeath playerDeath) => 
            _playerDeath = playerDeath;


        public void Initialize()
        {
            // M + 
            _playerDeath.OnDied += InvokeOnPlayerDied;
            _adsService.OnRewardedAdLoaded += InvokeOnRewardedAdLoaded;
        }

        public void Dispose()
        {
            _playerDeath.OnDied -= InvokeOnPlayerDied;
            _adsService.OnRewardedAdLoaded -= InvokeOnRewardedAdLoaded;
        }

        public void ShowRewardedAd(Action onRewardedAdFinished)
        {
            // M
            _adsService.ShowRewardedAd(() =>
            {
                // M
                _playerDeath.Revive();
                _enemySpawner.KillAllEnemies();
                _reviveInThisSession = true;
                
                onRewardedAdFinished?.Invoke();
            });
        }

        public bool TryShowInterstitialAd(Action onInterstitialAdFinished)
        {
            if (_adsService.IsInterstitialAdLoaded)
            {
                _adsService.ShowInterstitialAd(onInterstitialAdFinished);
                return true;
            }
            
            onInterstitialAdFinished?.Invoke();
            return false;
        }
        
        public async void ReloadScene()
        {
            // M
            _reviveInThisSession = false;
            // M
            await _sceneLoader.LoadAsync(SceneManager.GetActiveScene().buildIndex);
        }

        public bool CanRevive() => 
            _adsService.IsRewardedAdLoaded && !_reviveInThisSession;

        private void InvokeOnRewardedAdLoaded() => 
            OnRewardedAdLoaded?.Invoke();

        private void InvokeOnPlayerDied() => 
            OnPlayerDied?.Invoke();
    }
}
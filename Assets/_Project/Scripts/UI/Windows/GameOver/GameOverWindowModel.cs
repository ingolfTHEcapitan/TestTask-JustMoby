using System;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.Ads;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.SceneLoader;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.UI.Windows.GameOver
{
    public class GameOverWindowModel: IDisposable
    {
        public event Action OnPlayerDied;
        public event Action OnRewardedAdLoaded;
        
        private readonly IGamePauseService _pauseService;
        private readonly IAdsService _adsService;
        private readonly ISceneLoaderService _sceneLoader;
        private readonly PlayerDeath _playerDeath;
        private readonly EnemySpawner _enemySpawner;
        
        private bool _reviveInThisSession;
        
        public GameOverWindowModel(IGamePauseService pauseService, IAdsService adsService, ISceneLoaderService sceneLoader,
            EnemySpawner enemySpawner, PlayerDeath playerDeath)
        {
            _pauseService = pauseService;
            _adsService = adsService;
            _sceneLoader = sceneLoader;
            _enemySpawner = enemySpawner;
            _playerDeath = playerDeath;
        }
        
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

        public bool TryShowInterstitialAd(Action onInterstitialAdFinished = null)
        {
            if (_adsService.IsInterstitialAdLoaded)
            {
                _adsService.ShowInterstitialAd(onInterstitialAdFinished);
                return true;
            }
            
            onInterstitialAdFinished?.Invoke();
            return false;
        }

        public bool TryShowRewardedAd(Action onRewardedAdFinished = null)
        {
            if (!_adsService.IsRewardedAdLoaded)
            {
                onRewardedAdFinished?.Invoke();
                return false;
            }
            
            _adsService.ShowRewardedAd(() =>
            {
                _playerDeath.Revive();
                _enemySpawner.KillAllEnemies();
                _reviveInThisSession = true;
                onRewardedAdFinished?.Invoke();
            });
            
            return true;
        }

        public void SetPaused(bool paused) => 
            _pauseService.SetPaused(paused);

        public async UniTask ReloadSceneAsync()
        {
            _reviveInThisSession = false;
            await _sceneLoader.ReloadAsync();
        }

        public bool CanRevive() => 
            _adsService.IsRewardedAdLoaded && !_reviveInThisSession;

        private void InvokeOnRewardedAdLoaded() => 
            OnRewardedAdLoaded?.Invoke();

        private void InvokeOnPlayerDied() => 
            OnPlayerDied?.Invoke();
    }
}
using System;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Player.Components;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.Ads;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.SceneLoader;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.UI.Windows.GameOver
{
    public class GameOverWindowModel : IDisposable
    {
        public event Action OnPlayerDied;
        public event Action OnRewardedAdLoaded;
        
        private readonly IGamePauseService _pauseService;
        private readonly IAdsService _adsService;
        private readonly SceneLoaderService _sceneLoader;
        private readonly EnemySpawner _enemySpawner;
        private readonly LazyInject<PlayerContainer> _lazyPlayer;
        
        private  PlayerDeath _playerDeath;
        private bool _reviveInThisSession;
        
        public GameOverWindowModel(IGamePauseService pauseService, IAdsService adsService, SceneLoaderService sceneLoader,
            EnemySpawner enemySpawner, LazyInject<PlayerContainer> lazyPlayer)
        {
            _pauseService = pauseService;
            _adsService = adsService;
            _sceneLoader = sceneLoader;
            _enemySpawner = enemySpawner;
            _lazyPlayer = lazyPlayer;
        }
        
        public void Initialize()
        {
            _playerDeath = _lazyPlayer.Value.PlayerDeath;
            _playerDeath.OnDied += InvokeOnPlayerDied;
            _adsService.OnRewardedAdLoaded += InvokeOnRewardedAdLoaded;
        }

        public void Dispose()
        {
            _playerDeath.OnDied -= InvokeOnPlayerDied;
            _adsService.OnRewardedAdLoaded -= InvokeOnRewardedAdLoaded;
        }

        public async UniTask<bool> TryShowInterstitialAd() => 
            await _adsService.TryShowInterstitialAd();

        public async UniTask<bool> TryShowRewardedAd() => 
            await _adsService.TryShowRewardedAd();

        public void Revive()
        {
            _playerDeath.Revive();
            _enemySpawner.KillAllEnemies();
            _reviveInThisSession = true;
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
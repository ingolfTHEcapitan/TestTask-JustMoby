using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.Ads;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.GameOver
{
    public class GameOverWindow: MonoBehaviour, IWindow
    {   
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private Button _reviveButton;
        [SerializeField] private Button _loadSaveButton;
        
        private IGamePauseService _pauseService;
        private IAdsService _adsService;
        private PlayerDeath _playerDeath;
        private EnemySpawner _enemySpawner;
        private CursorController _cursorController;
        private bool _reviveInThisSession;
        private ISceneLoaderService _sceneLoader;

        [Inject]
        private void Construct(IGamePauseService pauseService, IAdsService adsService, 
            CursorController cursorController, ISceneLoaderService sceneLoader)
        {
            _cursorController = cursorController;
            _pauseService = pauseService;
            _adsService = adsService;
            _sceneLoader = sceneLoader;
        }

        public void Initialize(PlayerDeath playerDeath, EnemySpawner enemySpawner)
        {
            _playerDeath = playerDeath;
            _enemySpawner = enemySpawner;
            _playerDeath.OnDied += Open;
            _adsService.OnRewardedAdLoaded += RefreshReviveButtonState;
            _reviveButton.onClick.AddListener(OnReviveButtonClicked);
            _loadSaveButton.onClick.AddListener(OnLoadSaveButtonClicked);
            
            RefreshReviveButtonState();
        }

        private void OnDestroy()
        {
            _playerDeath.OnDied -= Open;
            _adsService.OnRewardedAdLoaded -= RefreshReviveButtonState;
            _reviveButton.onClick.RemoveListener(OnReviveButtonClicked);
            _loadSaveButton.onClick.RemoveListener(OnLoadSaveButtonClicked);
        }

        private async void OnReviveButtonClicked()
        {
            await CloseAsync();
            _cursorController.SetCursorVisible(true);
            
            _adsService.ShowRewardedAd(() =>
            {
                _playerDeath.Revive();
                _enemySpawner.KillAllEnemies();
                _reviveInThisSession = true;
                _cursorController.SetCursorVisible(false);
            });
        }

        private async void OnLoadSaveButtonClicked()
        {
            await CloseAsync();
            _cursorController.SetCursorVisible(true);

            if (_adsService.IsInterstitialAdLoaded)
                _adsService.ShowInterstitialAd(ReloadScene);
            else
                ReloadScene();
        }

        private async void ReloadScene()
        {
            _reviveInThisSession = false;
            _cursorController.SetCursorVisible(false);
            await _sceneLoader.LoadAsync(SceneManager.GetActiveScene().buildIndex);
        }

        private void RefreshReviveButtonState()
        {
            if (_adsService.IsRewardedAdLoaded && !_reviveInThisSession)
            {
                _reviveButton.interactable = true;
                return;
            }

            if (_reviveInThisSession) 
                _reviveButton.interactable = false;

        }

        public void Open()
        {
            _pauseService.SetPaused(true);
            _gameOverPanel.SetActive(true);
            _windowAnimation.AnimateOpen();
        }

        private async UniTask CloseAsync()
        {
            await _windowAnimation.AnimateCloseAsync();
            _pauseService.SetPaused(false);
            _gameOverPanel.SetActive(false);
        }
    }
}
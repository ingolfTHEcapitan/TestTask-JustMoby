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
    {   // V
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _windowContent;
        [SerializeField] private Button _reviveButton;
        [SerializeField] private Button _loadSaveButton;
        
        // M +
        private IGamePauseService _pauseService;
        private IAdsService _adsService;
        private PlayerDeath _playerDeath;
        private EnemySpawner _enemySpawner;
        
        //P
        private CursorController _cursorController;
        
        // M +
        private bool _reviveInThisSession;
        private ISceneLoaderService _sceneLoader;

        [Inject]
        private void Construct(IGamePauseService pauseService, IAdsService adsService, 
            CursorController cursorController, ISceneLoaderService sceneLoader)
        {
            // P
            _cursorController = cursorController;
            
            // M +
            _pauseService = pauseService;
            _adsService = adsService;
            _sceneLoader = sceneLoader;
        }

        // V 
        public void Initialize(PlayerDeath playerDeath, EnemySpawner enemySpawner)
        {
            // V
            _windowContent.SetActive(false);
            
            // M +
            _playerDeath = playerDeath;
            _enemySpawner = enemySpawner;
            
            // M + 
            _playerDeath.OnDied += Open;
            _adsService.OnRewardedAdLoaded += RefreshReviveButtonState;
            
            // V
            _reviveButton.onClick.AddListener(OnReviveButtonClicked);
            _loadSaveButton.onClick.AddListener(OnLoadSaveButtonClicked);
            
            // P
            RefreshReviveButtonState();
        }

        private void OnDestroy()
        {
            // M +
            _playerDeath.OnDied -= Open;
            _adsService.OnRewardedAdLoaded -= RefreshReviveButtonState;
            
            // V
            _reviveButton.onClick.RemoveListener(OnReviveButtonClicked);
            _loadSaveButton.onClick.RemoveListener(OnLoadSaveButtonClicked);
        }

        // P
        private async void OnReviveButtonClicked()
        {
            // V
            await CloseAsync();
            // P
            _cursorController.SetCursorVisible(true);
            
            // M +
            _adsService.ShowRewardedAd(() =>
            {
                // M+
                _playerDeath.Revive();
                _enemySpawner.KillAllEnemies();
                _reviveInThisSession = true;
                
                // P
                _cursorController.SetCursorVisible(false);
            });
        }
        
        // P
        private async void OnLoadSaveButtonClicked()
        {
            // V
            await CloseAsync();
            // P
            _cursorController.SetCursorVisible(true);

            // P
            if (_adsService.IsInterstitialAdLoaded)
                // M
                _adsService.ShowInterstitialAd(ReloadScene);
            else
            // M
                ReloadScene();
        }

        // M +
        private async void ReloadScene()
        {
            // M +
            _reviveInThisSession = false;
            // P
            _cursorController.SetCursorVisible(false);
            // M +
            await _sceneLoader.LoadAsync(SceneManager.GetActiveScene().buildIndex);
        }

        // P
        private void RefreshReviveButtonState()
        {
            // M+
            if (_adsService.IsRewardedAdLoaded && !_reviveInThisSession)
            {
                // V
                _reviveButton.interactable = true;
                return;
            }

            // Этот кусок уйдет по идее его заметит верхний
            // M
            if (_reviveInThisSession) 
                // V
                _reviveButton.interactable = false;
        }

        // V
        public void Open()
        {
            _pauseService.SetPaused(true);
            _windowContent.SetActive(true);
            _windowAnimation.AnimateOpen();
        }

        // V
        private async UniTask CloseAsync()
        {
            await _windowAnimation.AnimateCloseAsync();
            _pauseService.SetPaused(false);
            _windowContent.SetActive(false);
        }
    }
}
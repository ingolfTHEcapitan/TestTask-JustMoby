using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.Ads;
using _Project.Scripts.Services.GamePause;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.GameOver
{
    public class GameOverWindow: MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private Button _reviveButton;
        [SerializeField] private Button _loadSaveButton;
        
        private IGamePauseService _pauseService;
        private IAdsService _adsService;
        private PlayerDeath _playerDeath;
        private EnemySpawner _enemySpawner;
        private bool _reviveInThisSession;

        [Inject]
        public void Construct(IGamePauseService pauseService, IAdsService adsService)
        {
            _pauseService = pauseService;
            _adsService = adsService;
        }

        public void Initialize(PlayerDeath playerDeath, EnemySpawner enemySpawner)
        {
            _playerDeath = playerDeath;
            _enemySpawner = enemySpawner;
            _playerDeath.OnDied += ShowPanel;
            _adsService.OnRewardedAdLoaded += RefreshReviveButtonState;
            _reviveButton.onClick.AddListener(OnReviveButtonClicked);
            _loadSaveButton.onClick.AddListener(OnLoadSaveButtonClicked);
            
            RefreshReviveButtonState();
        }

        private void OnDestroy()
        {
            _playerDeath.OnDied -= ShowPanel;
            _adsService.OnRewardedAdLoaded -= RefreshReviveButtonState;
            _reviveButton.onClick.RemoveListener(OnReviveButtonClicked);
            _loadSaveButton.onClick.RemoveListener(OnLoadSaveButtonClicked);
        }

        private void OnReviveButtonClicked()
        {
            HidePanel();
            CursorController.SetCursorVisible(true);
            
            _adsService.ShowRewardedAd(() =>
            {
                _playerDeath.Revive();
                _enemySpawner.KillAllEnemies();
                _reviveInThisSession = true;
                CursorController.SetCursorVisible(false);
            });
        }

        private void OnLoadSaveButtonClicked()
        {
            HidePanel();
            CursorController.SetCursorVisible(true);

            if (_adsService.IsInterstitialAdLoaded)
                _adsService.ShowInterstitialAd(ReloadScene);
            else
                ReloadScene();
        }

        private void ReloadScene()
        {
            _reviveInThisSession = false;
            CursorController.SetCursorVisible(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

        private void ShowPanel()
        {
            _pauseService.SetPaused(true);
            _gameOverPanel.SetActive(true);
        }

        private void HidePanel()
        {
            _pauseService.SetPaused(false);
            _gameOverPanel.SetActive(false);    
        }
    }
}
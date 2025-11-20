using _Project.Scripts.Infrastructure.Services.GamePause;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Player;
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
        private PlayerDeath _playerDeath;
        private EnemySpawner _enemySpawner;
        
        [Inject]
        public void Construct(IGamePauseService pauseService, EnemySpawner enemySpawner)
        {
            _enemySpawner = enemySpawner;
            _pauseService = pauseService;
        }

        public void Initialize(PlayerDeath playerDeath)
        {
            _playerDeath = playerDeath;
            _playerDeath.OnDied += ShowPanel;
            _reviveButton.onClick.AddListener(OnReviveButtonClicked);
            _loadSaveButton.onClick.AddListener(OnLoadSaveButtonClicked);
        }

        private void OnDestroy()
        {
            _playerDeath.OnDied -= ShowPanel;
            _reviveButton.onClick.RemoveListener(OnReviveButtonClicked);
            _loadSaveButton.onClick.RemoveListener(OnLoadSaveButtonClicked);
        }

        private void OnReviveButtonClicked()
        {
            HidePanel();
            _playerDeath.Revive();
            _enemySpawner.KillAllEnemies();
        }

        private void OnLoadSaveButtonClicked()
        {
            HidePanel();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
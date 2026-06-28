using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.Sound;
using _Project.Scripts.UI.Windows.Shop;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.MainMenu
{
    public class MainMenuWindow: MonoBehaviour
    {
        private const string GameplayScene = "Gameplay";
        
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _exitButton;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _backgroundMusic;
        
        private ILoadingCurtainService _loadingCurtain;
        private IAudioService _audioService;
        private ShopWindow _shopWindow;

        [Inject]
        private void Construct(ILoadingCurtainService loadingCurtain, IAudioService audioService)
        {
            _loadingCurtain = loadingCurtain;
            _audioService = audioService;
        }

        public void Initialize(ShopWindow shopWindow)
        {
            _playButton.onClick.AddListener(StartGame);
            _shopButton.onClick.AddListener(OpenShopWindow);
            _exitButton.onClick.AddListener(ExitGame);
            
            _shopWindow = shopWindow;
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(StartGame);
            _shopButton.onClick.RemoveListener(OpenShopWindow);
            _exitButton.onClick.RemoveListener(ExitGame);
        }

        public void PlayBackGroundMusic()
        {
            _audioService.Play(_backgroundMusic, _audioSource);
        }

        private void OpenShopWindow() => 
            _shopWindow.Open();

        private void StartGame()
        {
            CursorController.SetCursorVisible(visible: false);
            _audioService.Stop(_audioSource);
            _loadingCurtain.ShowLoading();
            SceneManager.LoadSceneAsync(GameplayScene);
        }

        private void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.Services.Sound;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Windows.Settings;
using _Project.Scripts.UI.Windows.Shop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.MainMenu
{
    public class MainMenuWindow: MonoBehaviour
    {
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
        private SettingsView _settingsView;
        private CursorController _cursorController;
        private ISceneLoaderService _sceneLoader;

        [Inject]
        private void Construct(ILoadingCurtainService loadingCurtain, IAudioService audioService, 
            CursorController cursorController, ISceneLoaderService sceneLoader)
        {
            _cursorController = cursorController;
            _loadingCurtain = loadingCurtain;
            _audioService = audioService;
            _sceneLoader = sceneLoader;
        }

        public void Initialize(ShopWindow shopWindow, SettingsView settingsView)
        {
            _playButton.onClick.AddListener(StartGame);
            _settingsButton.onClick.AddListener(OpenSettingsWindow);
            _shopButton.onClick.AddListener(OpenShopWindow);
            _exitButton.onClick.AddListener(ExitGame);

            _shopWindow = shopWindow;
            _settingsView = settingsView;
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(StartGame);
            _settingsButton.onClick.RemoveListener(OpenSettingsWindow);
            _shopButton.onClick.RemoveListener(OpenShopWindow);
            _exitButton.onClick.RemoveListener(ExitGame);
            _audioService.Stop(_audioSource);
        }

        public void PlayBackGroundMusic()
        {
            _audioService.Play(_backgroundMusic, _audioSource);
        }

        private void OpenSettingsWindow() => 
            _settingsView.Open();

        private void OpenShopWindow() => 
            _shopWindow.Open();

        private async void StartGame()
        {
            _cursorController.SetCursorVisible(visible: false);
            _audioService.Stop(_audioSource);
            await _loadingCurtain.ShowLoadingAsync();
            await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.Gameplay);
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
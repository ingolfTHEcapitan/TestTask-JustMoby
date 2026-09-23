using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.Services.Sound;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Windows.LoadingCurtain;
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
        
        private LoadingCurtainPresenter _loadingCurtainPresenter;
        private IAudioService _audioService;
        private ShopWindowPresenter _shopWindowPresenter;
        private SettingsWindowPresenter _settingsWindowPresenter;
        private CursorController _cursorController;
        private ISceneLoaderService _sceneLoader;

        [Inject]
        private void Construct(LoadingCurtainPresenter loadingCurtain, IAudioService audioService, 
            CursorController cursorController, ISceneLoaderService sceneLoader)
        {
            _cursorController = cursorController;
            _loadingCurtainPresenter = loadingCurtain;
            _audioService = audioService;
            _sceneLoader = sceneLoader;
        }

        public void Initialize(ShopWindowPresenter shopWindowPresenter, SettingsWindowPresenter settingsWindowPresenter)
        {
            _playButton.onClick.AddListener(StartGame);
            _settingsButton.onClick.AddListener(OpenSettingsWindow);
            _shopButton.onClick.AddListener(OpenShopWindow);
            _exitButton.onClick.AddListener(ExitGame);

            _shopWindowPresenter = shopWindowPresenter;
            _settingsWindowPresenter = settingsWindowPresenter;
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
            _settingsWindowPresenter.Open();

        private void OpenShopWindow() => 
            _shopWindowPresenter.Open();

        private async void StartGame()
        {
            _cursorController.SetCursorVisible(visible: false);
            _audioService.Stop(_audioSource);
            _loadingCurtainPresenter.ShowLoading();
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
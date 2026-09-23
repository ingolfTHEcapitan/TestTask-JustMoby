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
        //V+
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _exitButton;
        //V+
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _backgroundMusic;
        
        //P+
        private LoadingCurtainPresenter _loadingCurtainPresenter;
        private ShopWindowPresenter _shopWindowPresenter;
        private SettingsWindowPresenter _settingsWindowPresenter;
        private CursorController _cursorController;
        //V+
        private IAudioService _audioService;
        //M+
        private ISceneLoaderService _sceneLoader;

        [Inject] //M+ //V+ //P
        private void Construct(LoadingCurtainPresenter loadingCurtain, IAudioService audioService, 
            CursorController cursorController, ISceneLoaderService sceneLoader)
        {
            //P+
            _cursorController = cursorController;
            _loadingCurtainPresenter = loadingCurtain;
            //V+
            _audioService = audioService;
            //M+
            _sceneLoader = sceneLoader;
        }

        //V+ //P+
        public void Initialize(ShopWindowPresenter shopWindowPresenter, SettingsWindowPresenter settingsWindowPresenter)
        {
            //V+
            _playButton.onClick.AddListener(StartGame);
            _settingsButton.onClick.AddListener(OpenSettingsWindow);
            _shopButton.onClick.AddListener(OpenShopWindow);
            _exitButton.onClick.AddListener(ExitGame);
            //P+
            _shopWindowPresenter = shopWindowPresenter;
            _settingsWindowPresenter = settingsWindowPresenter;
        }

        //V+
        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(StartGame);
            _settingsButton.onClick.RemoveListener(OpenSettingsWindow);
            _shopButton.onClick.RemoveListener(OpenShopWindow);
            _exitButton.onClick.RemoveListener(ExitGame);
            _audioService.Stop(_audioSource);
        }

        //V+
        public void PlayBackGroundMusic()
        {
            _audioService.Play(_backgroundMusic, _audioSource);
        }

        //P+
        private void OpenSettingsWindow() => 
            _settingsWindowPresenter.Open();

        //P+
        private void OpenShopWindow() => 
            _shopWindowPresenter.Open();

        //M+ //V+ //P
        private async void StartGame()
        {
            //P+
            _cursorController.SetCursorVisible(visible: false);
            //V+
            _audioService.Stop(_audioSource);
            //P+
            _loadingCurtainPresenter.ShowLoading();
            //M+
            await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.Gameplay);
        }

        //M+
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
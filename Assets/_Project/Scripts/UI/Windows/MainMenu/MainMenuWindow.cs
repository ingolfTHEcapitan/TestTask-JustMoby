using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.UI.Windows.Shop;
using UnityEditor;
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
        
        private ILoadingCurtainService _loadingCurtain;
        private ShopWindow _shopWindow;

        [Inject]
        private void Construct(ILoadingCurtainService loadingCurtain) => 
            _loadingCurtain = loadingCurtain;

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

        private void OpenShopWindow() => 
            _shopWindow.Open();

        private void StartGame()
        {
            CursorController.SetCursorVisible(visible: false);
            _loadingCurtain.ShowLoading();
            SceneManager.LoadSceneAsync(GameplayScene);
        }

        private void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
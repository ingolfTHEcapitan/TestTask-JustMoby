using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI
{
    public class HeadUpDisplay: MonoBehaviour
    {
        // V +
        [SerializeField] private Button _backMainMenuButton;
        [field:SerializeField] public Button OpenStatsWindowButton { get; private set; }
        [field:SerializeField] public HealthBarView HealthBarView { get; private set; }
        
        //P +
        private IInputService _inputService;
        private CursorController _cursorController;
        
        //M +
        private ILoadingCurtainService _loadingCurtain;
        private ISceneLoaderService _sceneLoader;

        [Inject]
        private void Construct(IInputService inputService, ILoadingCurtainService loadingCurtain, 
            CursorController cursorController, ISceneLoaderService sceneLoader)
        {
            // P +
            _cursorController = cursorController;
            _inputService = inputService;
            
            // M+
            _loadingCurtain = loadingCurtain;
            _sceneLoader = sceneLoader;
        }

        //V+
        public void Initialize() => 
            _backMainMenuButton.onClick.AddListener(BackToMainMenu);

        // V+
        private void OnDestroy() => 
            _backMainMenuButton.onClick.RemoveListener(BackToMainMenu);

        private void Update()
        {
            // P+
            if (_inputService.IsMainMenuButtonPressed())
                BackToMainMenu();
        }

        private async void BackToMainMenu()
        {
            // P +
            _cursorController.SetCursorVisible(visible: false);
            
            // M +
            await _loadingCurtain.ShowLoadingAsync();
            await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.MainMenu);
        }
    }
}
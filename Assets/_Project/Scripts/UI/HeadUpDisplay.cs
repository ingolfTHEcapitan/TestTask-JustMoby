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
        [SerializeField] private Button _backMainMenuButton;
        [field:SerializeField] public Button OpenStatsWindowButton { get; private set; }
        [field:SerializeField] public HealthBarView HealthBarView { get; private set; }
        
        private IInputService _inputService;
        private ILoadingCurtainService _loadingCurtain;
        private CursorController _cursorController;
        private ISceneLoaderService _sceneLoader;

        [Inject]
        private void Construct(IInputService inputService, ILoadingCurtainService loadingCurtain, 
            CursorController cursorController, ISceneLoaderService sceneLoader)
        {
            _cursorController = cursorController;
            _inputService = inputService;
            _loadingCurtain = loadingCurtain;
            _sceneLoader = sceneLoader;
        }

        public void Initialize() => 
            _backMainMenuButton.onClick.AddListener(BackToMainMenu);

        private void OnDestroy() => 
            _backMainMenuButton.onClick.RemoveListener(BackToMainMenu);

        private void Update()
        {
            if (_inputService.IsMainMenuButtonPressed())
                BackToMainMenu();
        }

        private async void BackToMainMenu()
        {
            _cursorController.SetCursorVisible(visible: false);
            await _loadingCurtain.ShowLoadingAsync();
            await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.MainMenu);
        }
    }
}
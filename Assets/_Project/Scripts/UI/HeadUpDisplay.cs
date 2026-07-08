using System;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
        
        [Inject]
        public void Construct(IInputService inputService, ILoadingCurtainService loadingCurtain)
        {
            _inputService = inputService;
            _loadingCurtain = loadingCurtain;
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
            CursorController.SetCursorVisible(visible: false);
            await _loadingCurtain.ShowLoading();
            SceneManager.LoadSceneAsync(SceneName.MainMenu);
        }
    }
}
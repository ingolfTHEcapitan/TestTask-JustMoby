using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Windows.PlayerStats;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayPresenter
    {
        private readonly IInputService _inputService;
        private readonly CursorController _cursorController;
        private readonly PlayerStatsWindowPresenter _playerStatsWindowPresenter;

        private readonly HeadUpDisplayModel _model;
        private readonly HeadUpDisplayView _view;

        public HeadUpDisplayPresenter(HeadUpDisplayModel model, HeadUpDisplayView view, IInputService inputService, 
            CursorController cursorController, PlayerStatsWindowPresenter playerStatsWindowPresenter)
        {
            _model = model;
            _view = view;
            _inputService = inputService;
            _cursorController = cursorController;
            _playerStatsWindowPresenter = playerStatsWindowPresenter;
        }

        public void Initialize()
        {
            _view.OnWindowDestroy += CleanUp;
            _view.OnBackToMainMenuButtonClicked += BackToOnMainMenu;
            _view.OnOpenStatsButtonClicked += OpenStatsWindow;
            _inputService.OnMainMenuButtonPressed += BackToOnMainMenu;
        }

        private void CleanUp()
        {
            _view.OnWindowDestroy -= CleanUp;
            _view.OnBackToMainMenuButtonClicked -= BackToOnMainMenu;
            _view.OnOpenStatsButtonClicked -= OpenStatsWindow;
            _inputService.OnMainMenuButtonPressed -= BackToOnMainMenu;
        }

        private void BackToOnMainMenu()
        {
            if (_model.GameIsPaused)
                return;
            
            _cursorController.SetCursorVisible(visible: false);
            _model.LoadMainMenuAsync().Forget(Debug.LogError);
        }

        private void OpenStatsWindow() => 
            _playerStatsWindowPresenter.Open();
    }
}
using System;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.UI.Common;
using Zenject;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayPresenter: IDisposable, ITickable
    {   //P +
        private IInputService _inputService;
        private CursorController _cursorController;
        
        private HeadUpDisplayView _view;
        private HeadUpDisplayModel _model;

        public HeadUpDisplayPresenter(HeadUpDisplayView view, HeadUpDisplayModel model, 
            IInputService inputService, CursorController cursorController)
        {
            _model = model;
            _view = view;
            _inputService = inputService;
            _cursorController = cursorController;
        }

        public void Initialize() => 
            _view.OnBackToMainMenuButtonClicked += BackToMainMenu;

        public void Dispose() => 
            _view.OnBackToMainMenuButtonClicked -= BackToMainMenu;

        public void Tick()
        {
            // P
            if (_inputService.IsMainMenuButtonPressed())
                BackToMainMenu();
        }

        private void BackToMainMenu()
        {
            // P +
            _cursorController.SetCursorVisible(visible: false);
        }
    }
}
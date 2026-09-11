using System;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.UI.Common;
using Zenject;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayPresenter: IDisposable, ITickable
    {
        private readonly IInputService _inputService;
        private readonly CursorController _cursorController;

        private readonly HeadUpDisplayModel _model;
        private HeadUpDisplayView _view;

        public HeadUpDisplayPresenter(HeadUpDisplayModel model, IInputService inputService, CursorController cursorController)
        {
            _model = model;
            _inputService = inputService;
            _cursorController = cursorController;
        }
        
        public void Construct(HeadUpDisplayView view) => 
            _view = view;

        public void Initialize() => 
            _view.OnBackToMainMenuButtonClicked += BackToMainMenu;

        public void Dispose() => 
            _view.OnBackToMainMenuButtonClicked -= BackToMainMenu;

        public void Tick()
        {
            if (_inputService.IsMainMenuButtonPressed())
                BackToMainMenu();
        }

        private async void BackToMainMenu()
        {
            _cursorController.SetCursorVisible(visible: false);
            await _model.LoadMainMenu();
        }
    }
}
using System;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayPresenter
    {
        private readonly IInputService _inputService;
        private readonly CursorController _cursorController;

        private readonly HeadUpDisplayModel _model;
        private readonly HeadUpDisplayView _view;

        public HeadUpDisplayPresenter(HeadUpDisplayModel model, HeadUpDisplayView view,
            IInputService inputService, CursorController cursorController)
        {
            _model = model;
            _view = view;
            _inputService = inputService;
            _cursorController = cursorController;
        }

        public void Initialize()
        {
            _view.OnWindowDestroy += CleanUp;
            _view.OnBackToMainMenuButtonClicked += BackToOnMainMenu;
            _inputService.OnMainMenuButtonPressed += BackToOnMainMenu;
        }

        private void CleanUp()
        {
            _view.OnWindowDestroy -= CleanUp;
            _view.OnBackToMainMenuButtonClicked -= BackToOnMainMenu;
            _inputService.OnMainMenuButtonPressed -= BackToOnMainMenu;
        }

        private void BackToOnMainMenu()
        {
            _cursorController.SetCursorVisible(visible: false);
            _model.LoadMainMenuAsync().Forget(Debug.LogError);
        }
    }
}
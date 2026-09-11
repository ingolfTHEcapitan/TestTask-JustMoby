using System;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.UI.Common;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayPresenter: IDisposable

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
        _view.OnBackToMainMenuButtonClicked += BackToOnMainMenu;
        _inputService.OnMainMenuButtonPressed += BackToOnMainMenu;
    }

    public void Dispose() =>
        _view.OnBackToMainMenuButtonClicked -= BackToOnMainMenu;

    private async void BackToOnMainMenu()
    {
        _cursorController.SetCursorVisible(visible: false);
        await _model.LoadMainMenu();
    }
    }
}
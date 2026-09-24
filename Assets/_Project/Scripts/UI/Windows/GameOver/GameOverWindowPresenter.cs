using System;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.GameOver
{
    public class GameOverWindowPresenter: IDisposable
    {
        private readonly GameOverWindowView _view;
        private readonly GameOverWindowModel _model;
        private readonly CursorController _cursorController;

        public GameOverWindowPresenter(GameOverWindowView view, GameOverWindowModel model, CursorController cursorController)
        {
            _model = model;
            _view = view;
            _cursorController = cursorController;
        }

        public void Initialize()
        {
            _view.Initialize();
            _model.Initialize();

            _model.OnPlayerDied += Open;
            _model.OnRewardedAdLoaded += RefreshReviveButtonState;

            _view.OnReviveButtonClicked += ShowRewardedAdAndReviveAnd;
            _view.OnLoadSaveButtonClicked += LoadSave;

            RefreshReviveButtonState();
        }

        public void Dispose()
        {
            _model.OnPlayerDied -= Open;
            _model.OnRewardedAdLoaded -= RefreshReviveButtonState;

            _view.OnReviveButtonClicked -= ShowRewardedAdAndReviveAnd;
            _view.OnLoadSaveButtonClicked -= LoadSave;
        }

        private async void LoadSave()
        {
            await _view.CloseAsync();
            _model.SetPaused(false);
            
            _cursorController.SetCursorVisible(true);
            _model.TryShowInterstitialAd();
            await ReloadScene();
        }

        private async UniTask ReloadScene()
        {
            _cursorController.SetCursorVisible(false);
           await _model.ReloadSceneAsync();
        }

        private async void ShowRewardedAdAndReviveAnd()
        {
            await _view.CloseAsync();
            _model.SetPaused(false);
            
            _cursorController.SetCursorVisible(true);

            _model.TryShowRewardedAd(() =>
            {
                _cursorController.SetCursorVisible(false);
                RefreshReviveButtonState();
            });
        }

        private void Open()
        {
            _model.SetPaused(true);
            _view.Open();
        }
        
        private void RefreshReviveButtonState()
        {
            bool canRevive = _model.CanRevive();
            _view.UpdateReviveButtonState(canRevive);
        }
    }
}
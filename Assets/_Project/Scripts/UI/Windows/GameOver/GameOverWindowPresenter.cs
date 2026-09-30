using System;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Windows.GameOver
{
    public class GameOverWindowPresenter: IDisposable
    {
        private readonly GameOverIWindowView _view;
        private readonly GameOverWindowModel _model;
        private readonly CursorController _cursorController;

        public GameOverWindowPresenter(GameOverIWindowView view, GameOverWindowModel model, CursorController cursorController)
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

            _view.OnReviveButtonClicked += ShowRewardedAdAndRevive;
            _view.OnLoadSaveButtonClicked += LoadSave;

            RefreshReviveButtonState();
        }

        public void Dispose()
        {
            _model.OnPlayerDied -= Open;
            _model.OnRewardedAdLoaded -= RefreshReviveButtonState;

            _view.OnReviveButtonClicked -= ShowRewardedAdAndRevive;
            _view.OnLoadSaveButtonClicked -= LoadSave;
        }

        private async void LoadSave()
        {
            try
            {
                await _view.CloseAsync();
                _model.SetPaused(false);
                _cursorController.SetCursorVisible(true);
                
                _model.TryShowInterstitialAd();
                _cursorController.SetCursorVisible(false);
                _model.ReloadSceneAsync().Forget(Debug.LogError);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private async void ShowRewardedAdAndRevive()
        {
            try
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
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private async void Open()
        {
            try
            {
                _model.SetPaused(true);
                await _view.OpenAsync();
                _cursorController.SetCursorVisible(true);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        private void RefreshReviveButtonState()
        {
            bool canRevive = _model.CanRevive();
            _view.UpdateReviveButtonState(canRevive);
        }
    }
}
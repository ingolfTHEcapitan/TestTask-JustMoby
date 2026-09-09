using System;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Windows.GameOver
{
    public class GameOverWindowPresenter
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

            _view.OnWindowDestroy += CleanUp;
            _view.OnReviveButtonClicked += ShowRewardedAdAndRevive;
            _view.OnLoadSaveButtonClicked += LoadSave;
            _model.OnRewardedAdLoaded += RefreshReviveButtonState;
            _model.OnPlayerDied += Open;

            RefreshReviveButtonState();
        }

        private void CleanUp()
        {
            _view.OnWindowDestroy -= CleanUp;
            _view.OnReviveButtonClicked -= ShowRewardedAdAndRevive;
            _view.OnLoadSaveButtonClicked -= LoadSave;
            _model.OnRewardedAdLoaded -= RefreshReviveButtonState;
            _model.OnPlayerDied -= Open;
            _model.Dispose();
        }

        private async void LoadSave()
        {
            try
            {
                await _view.CloseAsync();
                _model.SetPaused(false);
                _cursorController.SetCursorVisible(true);
                
                await _model.TryShowInterstitialAd();
                
                _cursorController.SetCursorVisible(false);
                await _model.ReloadSceneAsync();
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
                _cursorController.SetCursorVisible(true);

                bool isSuccess = await _model.TryShowRewardedAd();
                if (isSuccess)
                {
                    _model.Revive();
                    _model.SetPaused(false);
                    _cursorController.SetCursorVisible(false);
                }
                else
                {
                    _model.SetPaused(true);
                    await _view.OpenAsync();
                    _cursorController.SetCursorVisible(true);
                }
                
                RefreshReviveButtonState();
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
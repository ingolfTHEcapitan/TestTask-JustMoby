using System;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using _Project.Scripts.UI.Windows.Settings;
using _Project.Scripts.UI.Windows.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Windows.MainMenu
{
    public class MainMenuWindowPresenter: IDisposable
    {
        private readonly MainMenuWindowView _view;
        private readonly MainMenuWindowModel _model;
        private readonly LoadingCurtainPresenter _loadingCurtainPresenter;
        private readonly ShopWindowPresenter _shopWindowPresenter;
        private readonly SettingsWindowPresenter _settingsWindowPresenter;
        private readonly CursorController _cursorController;
        
        public MainMenuWindowPresenter(MainMenuWindowView view, MainMenuWindowModel model, LoadingCurtainPresenter loadingCurtainPresenter, ShopWindowPresenter shopWindowPresenter, SettingsWindowPresenter settingsWindowPresenter, CursorController cursorController)
        {
            _view = view;
            _model = model;
            _loadingCurtainPresenter = loadingCurtainPresenter;
            _shopWindowPresenter = shopWindowPresenter;
            _settingsWindowPresenter = settingsWindowPresenter;
            _cursorController = cursorController;
        }

        public void Initialize()
        {
            _view.Initialize();
            
            _view.OnPlayButtonClicked += StartGame;
            _view.OnSettingsButtonClicked += OpenSettingsWindow;
            _view.OnShopButtonClicked += OpenShopWindow;
            _view.OnExitButtonClicked += ExitGame;
            
            _view.PlayBackgroundMusic();
            _cursorController.SetCursorVisible(true);
        }

        public void Dispose()
        {
            _view.OnPlayButtonClicked -= StartGame;
            _view.OnSettingsButtonClicked -= OpenSettingsWindow;
            _view.OnShopButtonClicked -= OpenShopWindow;
            _view.OnExitButtonClicked -= ExitGame;
        }

        private void StartGame()
        {
            _cursorController.SetCursorVisible(false);
            _loadingCurtainPresenter.ShowLoading();
            _view.StopBackgroundMusic();
            _model.LoadGameplaySceneAsync().Forget(Debug.LogError);
        }

        private void OpenSettingsWindow() => 
            _settingsWindowPresenter.OpenAsync().Forget(Debug.LogError);

        private void OpenShopWindow() => 
            _shopWindowPresenter.OpenAsync().Forget(Debug.LogError);

        private void ExitGame() => 
            _model.ExitGame();
    }
}
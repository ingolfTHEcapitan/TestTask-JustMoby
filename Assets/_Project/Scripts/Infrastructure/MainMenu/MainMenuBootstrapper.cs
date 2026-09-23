using System;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using _Project.Scripts.UI.Windows.MainMenu;
using _Project.Scripts.UI.Windows.SaveConflictResolve;
using _Project.Scripts.UI.Windows.Settings;
using _Project.Scripts.UI.Windows.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.MainMenu
{
    public class MainMenuBootstrapper: IInitializable, IDisposable
    {
        private readonly LoadingCurtainPresenter _loadingCurtainPresenter;
        private readonly IProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IUIFactory _uiFactory;
        private readonly ISaveConflictResolveService _saveConflictResolveService;

        private readonly Transform _uiParent;
        private readonly CursorController _cursorController;
        private readonly LazyInject<SettingsWindowPresenter> _lazySettingsWindowPresenter;
        private readonly LazyInject<ShopWindowPresenter> _lazyShopWindowPresenter;
        private SettingsWindowPresenter _settingsWindowPresenter;
        private ShopWindowPresenter _shopWindowPresenter;

        public MainMenuBootstrapper(LoadingCurtainPresenter loadingCurtainPresenter, IProgressService progressService,
            [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, IUIFactory uiFactory,
            ISaveConflictResolveService saveConflictResolveService, Transform uiParent, CursorController cursorController,
            LazyInject<SettingsWindowPresenter> lazySettingsWindowPresenter, LazyInject<ShopWindowPresenter> lazyShopWindowPresenter)
        {
            _lazyShopWindowPresenter = lazyShopWindowPresenter;
            _lazySettingsWindowPresenter = lazySettingsWindowPresenter;
            _saveConflictResolveService = saveConflictResolveService;
            _loadingCurtainPresenter = loadingCurtainPresenter;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _uiFactory = uiFactory;
            _uiParent = uiParent;
            _cursorController = cursorController;
        }

        public async void Initialize()
        {
            _saveConflictResolveService.Initialize();
            _progressService.PlayerProgress = await _saveLoadService.LoadProgressAsync();
            
            ShopWindowView shopWindowView = await _uiFactory.CreateShopWindowViewAsync(_uiParent);;
            _shopWindowPresenter = _lazyShopWindowPresenter.Value;
            _shopWindowPresenter.Initialize();

            SettingsWindowView settingsWindowView = await _uiFactory.CreateSettingsViewAsync(_uiParent);
            _settingsWindowPresenter = _lazySettingsWindowPresenter.Value;
            _settingsWindowPresenter.Initialize();
            
            MainMenuWindow mainMenu = await InitMainMenu();

            _cursorController.SetCursorVisible(visible: true);
            mainMenu.PlayBackGroundMusic();
            _loadingCurtainPresenter.HideLoading();
        }

        public void Dispose()
        {
            _settingsWindowPresenter.Dispose();
            _shopWindowPresenter.Dispose();
        }

        private async UniTask<MainMenuWindow> InitMainMenu()
        {
            MainMenuWindow mainMenu = await _uiFactory.CreateMainMenuWindowAsync(_uiParent);
            mainMenu.Initialize(_shopWindowPresenter, _settingsWindowPresenter);
            return mainMenu;
        }
    }
}
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
using _Project.Scripts.UI.Windows.Shop.Item;
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
        private readonly ShopItemUIFactory _shopItemUIFactory;
        private readonly CursorController _cursorController;
        private readonly LazyInject<SettingsWindowPresenter> _lazySettingsWindowPresenter;
        private SettingsWindowPresenter _settingsWindowPresenter;

        public MainMenuBootstrapper(LoadingCurtainPresenter loadingCurtainPresenter, IProgressService progressService,
            [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, IUIFactory uiFactory,
            ISaveConflictResolveService saveConflictResolveService, Transform uiParent, CursorController cursorController,
            LazyInject<SettingsWindowPresenter> lazySettingsWindowPresenter, ShopItemUIFactory shopItemUIFactory)
        {
            _lazySettingsWindowPresenter = lazySettingsWindowPresenter;
            _saveConflictResolveService = saveConflictResolveService;
            _loadingCurtainPresenter = loadingCurtainPresenter;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _uiFactory = uiFactory;
            _uiParent = uiParent;
            _cursorController = cursorController;
            _shopItemUIFactory = shopItemUIFactory;
        }

        public async void Initialize()
        {
            _saveConflictResolveService.Initialize();
            _progressService.PlayerProgress = await _saveLoadService.LoadProgressAsync();
            
            ShopWindow shopWindow = await InitShopWindow();
            
            SettingsWindowView settingsWindowView = await InitSettingsView();
            _settingsWindowPresenter = _lazySettingsWindowPresenter.Value;
            _settingsWindowPresenter.Initialize();
            
            MainMenuWindow mainMenu = await InitMainMenu(shopWindow, settingsWindowView);

            _cursorController.SetCursorVisible(visible: true);
            mainMenu.PlayBackGroundMusic();
            _loadingCurtainPresenter.HideLoading();
        }

        public void Dispose() => 
            _settingsWindowPresenter.Dispose();

        private async UniTask<SettingsWindowView> InitSettingsView()
        {
            SettingsWindowView settingsWindowView = await _uiFactory.CreateSettingsViewAsync(_uiParent);
            settingsWindowView.Initialize();
            return settingsWindowView;
        }

        private async UniTask<ShopWindow> InitShopWindow()
        {
            ShopWindow shopWindow = await _uiFactory.CreateShopWindowAsync(_uiParent);
            shopWindow.Initialize(_shopItemUIFactory);
            return shopWindow;
        }

        private async UniTask<MainMenuWindow> InitMainMenu(ShopWindow shopWindow, SettingsWindowView settingsWindowView)
        {
            MainMenuWindow mainMenu = await _uiFactory.CreateMainMenuWindowAsync(_uiParent);
            mainMenu.Initialize(shopWindow, settingsWindowView);
            return mainMenu;
        }
    }
}
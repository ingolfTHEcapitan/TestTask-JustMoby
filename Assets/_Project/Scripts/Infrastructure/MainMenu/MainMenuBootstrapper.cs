using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveConflictResolve;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using _Project.Scripts.UI.Windows.MainMenu;
using _Project.Scripts.UI.Windows.Settings;
using _Project.Scripts.UI.Windows.Shop;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.MainMenu
{
    public class MainMenuBootstrapper: IInitializable
    {
        private readonly ILoadingCurtainService _loadingCurtain;
        private readonly IProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IUIFactory _uiFactory;
        private readonly ISaveConflictResolveService _saveConflictResolveService;

        private readonly Transform _uiParent;
        private readonly CursorController _cursorController;
        private readonly SettingsPresenter _settingsPresenter;

        public MainMenuBootstrapper(ILoadingCurtainService loadingCurtain, IProgressService progressService,
            [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, IUIFactory uiFactory,
            ISaveConflictResolveService saveConflictResolveService, Transform uiParent, CursorController cursorController,
            SettingsPresenter settingsPresenter)
        {
            _settingsPresenter = settingsPresenter;
            _saveConflictResolveService = saveConflictResolveService;
            _loadingCurtain = loadingCurtain;
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
           
            GameObject mainMenuLayer = await _uiFactory.CreateMainMenuLayerAsync(_uiParent);
            
            ShopWindow shopWindow = InitShopWindow(mainMenuLayer);
            SettingsView settingsView = InitSettingsWindow(mainMenuLayer);
            MainMenuWindow mainMenu = InitMainMenu(mainMenuLayer, shopWindow, settingsView);

            _cursorController.SetCursorVisible(visible: true);
            mainMenu.PlayBackGroundMusic();
            _loadingCurtain.HideLoading();
        }

        private SettingsView InitSettingsWindow(GameObject mainMenuLayer)
        {
            SettingsView settingsView = mainMenuLayer.GetComponentInChildren<SettingsView>(includeInactive: true);
            settingsView.Initialize();
            _settingsPresenter.Construct(settingsView);
            _settingsPresenter.Initialize();
            return settingsView;
        }

        private ShopWindow InitShopWindow(GameObject mainMenuLayer)
        {
            ShopWindow shopWindow = mainMenuLayer.GetComponentInChildren<ShopWindow>(includeInactive: true);
            shopWindow.Initialize();
            return shopWindow;
        }

        private MainMenuWindow InitMainMenu(GameObject mainMenuLayer, 
            ShopWindow shopWindow, SettingsView settingsView)
        {
            MainMenuWindow mainMenu = mainMenuLayer.GetComponentInChildren<MainMenuWindow>();
            mainMenu.Initialize(shopWindow, settingsView);
            return mainMenu;
        }
    }
}
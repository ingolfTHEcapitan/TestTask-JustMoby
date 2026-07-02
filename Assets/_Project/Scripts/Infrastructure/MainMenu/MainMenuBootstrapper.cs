using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveConflictResolve;
using _Project.Scripts.Services.SaveLoad;
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

        public MainMenuBootstrapper(ILoadingCurtainService loadingCurtain, IProgressService progressService,
            [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, IUIFactory uiFactory,
            ISaveConflictResolveService saveConflictResolveService, Transform uiParent)
        {
            _saveConflictResolveService = saveConflictResolveService;
            _loadingCurtain = loadingCurtain;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _uiFactory = uiFactory;
            _uiParent = uiParent;
        }

        public async void Initialize()
        {
            await _saveConflictResolveService.CreateWindow();
            _progressService.PlayerProgress = await _saveLoadService.LoadProgressAsync();
           
            GameObject mainMenuLayer = await _uiFactory.CreateMainMenuLayer(_uiParent);
            
            ShopWindow shopWindow = InitShopWindow(mainMenuLayer);
            SettingsWindow settingsWindow = InitSettingsWindow(mainMenuLayer);
            MainMenuWindow mainMenu = InitMainMenu(mainMenuLayer, shopWindow, settingsWindow);

            CursorController.SetCursorVisible(visible: true);
            mainMenu.PlayBackGroundMusic();
            _loadingCurtain.HideLoading();
        }

        private SettingsWindow InitSettingsWindow(GameObject mainMenuLayer)
        {
            SettingsWindow settingsWindow = mainMenuLayer.GetComponentInChildren<SettingsWindow>(includeInactive: true);
            settingsWindow.Initialize();
            return settingsWindow;
        }

        private ShopWindow InitShopWindow(GameObject mainMenuLayer)
        {
            ShopWindow shopWindow = mainMenuLayer.GetComponentInChildren<ShopWindow>(includeInactive: true);
            shopWindow.Initialize();
            return shopWindow;
        }

        private MainMenuWindow InitMainMenu(GameObject mainMenuLayer, 
            ShopWindow shopWindow, SettingsWindow settingsWindow)
        {
            MainMenuWindow mainMenu = mainMenuLayer.GetComponentInChildren<MainMenuWindow>();
            mainMenu.Initialize(shopWindow, settingsWindow);
            return mainMenu;
        }
    }
}
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.Factory.UIFactory;
using _Project.Scripts.Services.LoadingScreen;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Windows.MainMenu;
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
        private readonly Transform _uiParent;

        public MainMenuBootstrapper(ILoadingCurtainService loadingCurtain, IProgressService progressService,
            [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, IUIFactory uiFactory, Transform uiParent)
        {
            _loadingCurtain = loadingCurtain;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _uiFactory = uiFactory;
            _uiParent = uiParent;
        }

        public async void Initialize()
        {
            _progressService.PlayerProgress = await _saveLoadService.LoadProgressAsync();
           
            GameObject mainMenuLayer = await _uiFactory.CreateMainMenuLayer(_uiParent);
            
            ShopWindow shopWindow = InitShopWindow(mainMenuLayer);
            InitMainMenu(mainMenuLayer, shopWindow);

            CursorController.SetCursorVisible(visible: true);
            _loadingCurtain.HideLoading();
        }

        private ShopWindow InitShopWindow(GameObject mainMenuLayer)
        {
            ShopWindow shopWindow = mainMenuLayer.GetComponentInChildren<ShopWindow>(includeInactive: true);
            shopWindow.Initialize();
            return shopWindow;
        }

        private void InitMainMenu(GameObject mainMenuLayer, ShopWindow shopWindow)
        {
            MainMenuWindow mainMenu = mainMenuLayer.GetComponentInChildren<MainMenuWindow>();
            mainMenu.Initialize(shopWindow);
        }
    }
}
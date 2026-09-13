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
    public class MainMenuBootstrapper: IInitializable
    {
        private readonly LoadingCurtainPresenter _loadingCurtainPresenter;
        private readonly IProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IUIFactory _uiFactory;
        private readonly ISaveConflictResolveService _saveConflictResolveService;

        private readonly Transform _uiParent;
        private readonly CursorController _cursorController;
        private readonly SettingsWindowPresenter _settingsWindowPresenter;

        public MainMenuBootstrapper(LoadingCurtainPresenter loadingCurtainPresenter, IProgressService progressService,
            [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, IUIFactory uiFactory,
            ISaveConflictResolveService saveConflictResolveService, Transform uiParent, CursorController cursorController,
            SettingsWindowPresenter settingsWindowPresenter)
        {
            _settingsWindowPresenter = settingsWindowPresenter;
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
            
            ShopWindow shopWindow = await InitShopWindow();
            SettingsWindowView settingsWindowView = await InitSettingsView();
            InitSettingsPresenter(settingsWindowView);
            MainMenuWindow mainMenu = await InitMainMenu(shopWindow, settingsWindowView);

            _cursorController.SetCursorVisible(visible: true);
            mainMenu.PlayBackGroundMusic();
            _loadingCurtainPresenter.HideLoading();
        }

        private void InitSettingsPresenter(SettingsWindowView settingsWindowView)
        {
            _settingsWindowPresenter.Construct(settingsWindowView);
            _settingsWindowPresenter.Initialize();
        }

        private async UniTask<SettingsWindowView> InitSettingsView()
        {
            SettingsWindowView settingsWindowView = await _uiFactory.CreateSettingsViewAsync(_uiParent);
            settingsWindowView.Initialize();
            return settingsWindowView;
        }

        private async UniTask<ShopWindow> InitShopWindow()
        {
            ShopWindow shopWindow = await _uiFactory.CreateShopWindowAsync(_uiParent);
            shopWindow.Initialize();
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
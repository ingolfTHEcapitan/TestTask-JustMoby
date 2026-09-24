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

        private readonly Transform _uiParent;
        private readonly LazyInject<SettingsWindowPresenter> _lazySettingsWindowPresenter;
        private readonly LazyInject<ShopWindowPresenter> _lazyShopWindowPresenter;
        private readonly LazyInject<MainMenuWindowPresenter> _lazyMainMenuWindowPresenter;
        private readonly LazyInject<SaveConflictResolveWindowPresenter> _lazySaveConflictResolveWindowPresenter;
        private SettingsWindowPresenter _settingsWindowPresenter;
        private ShopWindowPresenter _shopWindowPresenter;
        private MainMenuWindowPresenter _mainMenuWindowPresenter;
        private SaveConflictResolveWindowPresenter _saveConflictResolveWindowPresenter;

        public MainMenuBootstrapper(LoadingCurtainPresenter loadingCurtainPresenter, IProgressService progressService,
            [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, IUIFactory uiFactory, Transform uiParent,
            LazyInject<SettingsWindowPresenter> lazySettingsWindowPresenter, LazyInject<ShopWindowPresenter> lazyShopWindowPresenter,
            LazyInject<MainMenuWindowPresenter> lazyMainMenuWindowPresenter,LazyInject<SaveConflictResolveWindowPresenter> lazySaveConflictResolveWindowPresenter)
        {
            _lazyShopWindowPresenter = lazyShopWindowPresenter;
            _lazySettingsWindowPresenter = lazySettingsWindowPresenter;
            _lazyMainMenuWindowPresenter = lazyMainMenuWindowPresenter;
            _lazySaveConflictResolveWindowPresenter = lazySaveConflictResolveWindowPresenter;
            _loadingCurtainPresenter = loadingCurtainPresenter;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _uiFactory = uiFactory;
            _uiParent = uiParent;
        }

        public async void Initialize()
        {
            await _uiFactory.CreateSaveConflictResolveWindowViewAsync();
            _saveConflictResolveWindowPresenter = _lazySaveConflictResolveWindowPresenter.Value;
            _saveConflictResolveWindowPresenter.Initialize();
            
            _progressService.PlayerProgress = await _saveLoadService.LoadProgressAsync();
            
            await _uiFactory.CreateShopWindowViewAsync(_uiParent);
            _shopWindowPresenter = _lazyShopWindowPresenter.Value;
            _shopWindowPresenter.Initialize();

            await _uiFactory.CreateSettingsViewAsync(_uiParent);
            _settingsWindowPresenter = _lazySettingsWindowPresenter.Value;
            _settingsWindowPresenter.Initialize();
            
            await _uiFactory.CreateMainMenuWindowViewAsync(_uiParent);
            _mainMenuWindowPresenter = _lazyMainMenuWindowPresenter.Value;
            _mainMenuWindowPresenter.Initialize();
            
            _loadingCurtainPresenter.HideLoading();
        }

        public void Dispose()
        {
            _settingsWindowPresenter.Dispose();
            _shopWindowPresenter.Dispose();
            _mainMenuWindowPresenter.Dispose();
        }
    }
}
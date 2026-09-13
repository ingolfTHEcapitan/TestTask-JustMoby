using System;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Authentication;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.Services.RemoteConfig;
using _Project.Scripts.Services.RemoteConfig.RemoteConfigFactory;
using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.UI.Factory;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.Infrastructure.Project
{
    public class ProjectBootstrapper: IInitializable, IDisposable
    {
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly IRemoteConfigFactory _remoteConfigFactory;
        private readonly IAuthService _authService;
        private readonly IAssetProvider _assetProvider;
        private readonly IAnalyticsService _analyticsService;
        private readonly ISceneLoaderService _sceneLoader;
        private readonly IUIFactory _uiFactory;
        private readonly LazyInject<IIAPService> _lazyIapService;
        private readonly LazyInject<LoadingCurtainPresenter> _lazyLoadingWindowPresenter;
        private IIAPService _iapService;


        public ProjectBootstrapper(LazyInject<LoadingCurtainPresenter> lazyLoadingWindowPresenter, IRemoteConfigService remoteConfigService, 
            IRemoteConfigFactory remoteConfigFactory, IAuthService authService, IAssetProvider assetProvider, IUIFactory uiFactory,
            IAnalyticsService analyticsService, LazyInject<IIAPService> lazyIapService, ISceneLoaderService sceneLoader)
        {
            _lazyIapService = lazyIapService;
            _lazyLoadingWindowPresenter = lazyLoadingWindowPresenter;
            _remoteConfigService = remoteConfigService;
            _remoteConfigFactory = remoteConfigFactory;
            _authService = authService;
            _assetProvider = assetProvider;
            _uiFactory = uiFactory;
            _analyticsService = analyticsService;
            _sceneLoader = sceneLoader;
        }

        public async void Initialize()
        {
            await _assetProvider.InitializeAsync();
            await _uiFactory.CreateLoadingCurtainViewAsync();
            _lazyLoadingWindowPresenter.Value.ShowLoading();
            
            UniTask analyticsServiceTask = _analyticsService.InitializeAsync();
            UniTask remoteConfigServiceTask = _remoteConfigService.FetchDataAsync();
            UniTask authServiceTask = _authService.SignUpAsync();

            await UniTask.WhenAll(analyticsServiceTask, remoteConfigServiceTask, authServiceTask);

            _remoteConfigFactory.ApplyRemoteConfigs();
            
            _iapService = _lazyIapService.Value;
            _iapService.Initialize();

            await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.MainMenu);
        }

        public void Dispose() => 
            _iapService.Dispose();
    }
}
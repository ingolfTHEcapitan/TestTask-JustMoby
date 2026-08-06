using System;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Authentication;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.RemoteConfig;
using _Project.Scripts.Services.RemoteConfig.RemoteConfigFactory;
using _Project.Scripts.Services.SceneLoader;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.Infrastructure.Project
{
    public class ProjectBootstrapper: IInitializable, IDisposable
    {
        private readonly ILoadingCurtainService _loadingCurtain;
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly IRemoteConfigFactory _remoteConfigFactory;
        private readonly IAuthService _authService;
        private readonly IAssetProvider _assetProvider;
        private readonly IAnalyticsService _analyticsService;
        private readonly ISceneLoaderService _sceneLoader;
        private readonly LazyInject<IIAPService> _lazyIapService;
        private IIAPService _iapService;


        public ProjectBootstrapper(ILoadingCurtainService loadingCurtain, IRemoteConfigService remoteConfigService, 
            IRemoteConfigFactory remoteConfigFactory, IAuthService authService, IAssetProvider assetProvider,
            IAnalyticsService analyticsService, LazyInject<IIAPService> lazyIapService, ISceneLoaderService sceneLoader)
        {
            _lazyIapService = lazyIapService;
            _loadingCurtain = loadingCurtain;
            _remoteConfigService = remoteConfigService;
            _remoteConfigFactory = remoteConfigFactory;
            _authService = authService;
            _assetProvider = assetProvider;
            _analyticsService = analyticsService;
            _sceneLoader = sceneLoader;
        }

        public async void Initialize()
        {
            UniTask assetProviderTask = _assetProvider.InitializeAsync();
            UniTask loadingCurtainTask = _loadingCurtain.ShowLoadingAsync();
            UniTask analyticsServiceTask = _analyticsService.InitializeAsync();
            UniTask remoteConfigServiceTask = _remoteConfigService.FetchDataAsyncAsync();
            UniTask authServiceTask = _authService.SignUpAsync();

            await UniTask.WhenAll(assetProviderTask, loadingCurtainTask, analyticsServiceTask, 
                remoteConfigServiceTask, authServiceTask);

            _remoteConfigFactory.ApplyRemoteConfigs();
            
            _iapService = _lazyIapService.Value;
            _iapService.Initialize();

            await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.MainMenu);
        }

        public void Dispose()
        {
            if (_iapService is IDisposable disposable) 
                disposable.Dispose();
        }
    }
}
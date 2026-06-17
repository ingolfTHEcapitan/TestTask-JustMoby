using System;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Authentication;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.RemoteConfig;
using _Project.Scripts.Services.RemoteConfig.RemoteConfigFactory;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.Scripts.Infrastructure.Project
{
    public class ProjectBootstrapper: IInitializable, IDisposable
    {
        private const string MainMenuScene = "MainMenu";
        
        private readonly ILoadingCurtainService _loadingCurtain;
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly IRemoteConfigFactory _remoteConfigFactory;
        private readonly LazyInject<IIAPService> _lazyIapService;
        private readonly IAuthService _authService;
        private readonly IAssetProvider _assetProvider;
        private IIAPService _iapService;
        private IAnalyticsService _analyticsService;
        
        public ProjectBootstrapper(ILoadingCurtainService loadingCurtain, LazyInject<IIAPService> lazyIapService, 
            IRemoteConfigService remoteConfigService, IRemoteConfigFactory remoteConfigFactory, 
            IAuthService authService, IAssetProvider assetProvider, IAnalyticsService analyticsService)
        {
            _lazyIapService = lazyIapService;
            _loadingCurtain = loadingCurtain;
            _remoteConfigService = remoteConfigService;
            _remoteConfigFactory = remoteConfigFactory;
            _authService = authService;
            _assetProvider = assetProvider;
            _analyticsService = analyticsService;
        }

        public async void Initialize()
        {
            await _assetProvider.InitializeAsync();
            await _loadingCurtain.ShowLoading();
            await _analyticsService.InitializeAsync();
            await _remoteConfigService.FetchDataAsync();
            _remoteConfigFactory.ApplyRemoteConfigs();
            await _authService.SignUpAsync();
            
            _iapService = _lazyIapService.Value;
            _iapService.Initialize();

            SceneManager.LoadSceneAsync(MainMenuScene);
        }

        public void Dispose()
        {
            if (_iapService is IDisposable disposable) 
                disposable.Dispose();
        }
    }
}
using System;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Services.Ads;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Authentication;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.Services.RemoteConfig;
using _Project.Scripts.Services.RemoteConfig.RemoteConfigFactory;
using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Project
{
    public class ProjectBootstrapper : IInitializable, IDisposable
    {
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly RemoteConfigApplier _remoteConfigApplier;
        private readonly IAuthService _authService;
        private readonly IAssetProvider _assetProvider;
        private readonly IAnalyticsService _analyticsService;
        private readonly SceneLoaderService _sceneLoader;
        private readonly UIFactory _uiFactory;
        private readonly IAdsService _adsService;
        private readonly LazyInject<IIAPService> _lazyIapService;
        private readonly LazyInject<LoadingCurtainPresenter> _lazyLoadingWindowPresenter;
        private IIAPService _iapService;


        public ProjectBootstrapper(LazyInject<LoadingCurtainPresenter> lazyLoadingWindowPresenter, IRemoteConfigService remoteConfigService, 
            RemoteConfigApplier remoteConfigApplier, IAuthService authService, IAssetProvider assetProvider, UIFactory uiFactory,
            IAnalyticsService analyticsService, LazyInject<IIAPService> lazyIapService, SceneLoaderService sceneLoader, IAdsService adsService)
        {
            _lazyIapService = lazyIapService;
            _lazyLoadingWindowPresenter = lazyLoadingWindowPresenter;
            _remoteConfigService = remoteConfigService;
            _remoteConfigApplier = remoteConfigApplier;
            _authService = authService;
            _assetProvider = assetProvider;
            _uiFactory = uiFactory;
            _analyticsService = analyticsService;
            _sceneLoader = sceneLoader;
            _adsService = adsService;
        }

        public void Initialize() => 
            InitializeAsync().Forget();

        private async UniTaskVoid InitializeAsync()
        {
            try
            {
                await _assetProvider.InitializeAsync();
                await _uiFactory.CreateLoadingCurtainViewAsync();
                _lazyLoadingWindowPresenter.Value.ShowLoading();
            
                await _analyticsService.InitializeAsync();
                UniTask remoteConfigServiceTask = _remoteConfigService.FetchDataAsync();
                UniTask authServiceTask = _authService.SignUpAsync();

                await UniTask.WhenAll(remoteConfigServiceTask, authServiceTask);

                _remoteConfigApplier.ApplyRemoteConfigs();
            
                _adsService.Initialize();
            
                _iapService = _lazyIapService.Value;
                _iapService.Initialize();

                await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.MainMenu);
            }
            catch (Exception e)
            {
                Debug.LogError($"[PROJECT BOOTSTRAPPER] init error: {e}");
            }
        }

        public void Dispose() => 
            _iapService.Dispose();
    }
}
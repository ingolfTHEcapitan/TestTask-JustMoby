using System;
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
        
        private LazyInject<IIAPService> _lazyIapService;
        private IIAPService _iapService;
        private IAuthService _authService;

        public ProjectBootstrapper(ILoadingCurtainService loadingCurtain, LazyInject<IIAPService> lazyIapService, 
            IRemoteConfigService remoteConfigService, IRemoteConfigFactory remoteConfigFactory, IAuthService authService)
        {
            _lazyIapService = lazyIapService;
            _loadingCurtain = loadingCurtain;
            _remoteConfigService = remoteConfigService;
            _remoteConfigFactory = remoteConfigFactory;
            _authService = authService;
        }

        public async void Initialize()
        {
            await _loadingCurtain.ShowLoading();
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
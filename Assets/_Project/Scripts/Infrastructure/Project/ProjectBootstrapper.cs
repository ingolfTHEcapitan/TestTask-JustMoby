using System;
using _Project.Scripts.Services.Factory.RemoteConfigFactory;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.Services.LoadingScreen;
using _Project.Scripts.Services.RemoteConfig;
using _Project.Scripts.Services.SaveLoad;
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

        private ISaveLoadService _saveLoadServiceInstance;
        private LazyInject<IIAPService> _lazyIapService;
        private IIAPService _iapService;

        public ProjectBootstrapper(ILoadingCurtainService loadingCurtain, LazyInject<IIAPService> lazyIapService, 
            IRemoteConfigService remoteConfigService, IRemoteConfigFactory remoteConfigFactory)
        {
            _lazyIapService = lazyIapService;
            _loadingCurtain = loadingCurtain;
            _remoteConfigService = remoteConfigService;
            _remoteConfigFactory = remoteConfigFactory;
        }

        public async void Initialize()
        {
            await _loadingCurtain.ShowLoading();
            await _remoteConfigService.FetchDataAsync();
            _remoteConfigFactory.ApplyRemoteConfigs();
            
            _iapService = _lazyIapService.Value;
            _iapService.Initialize();

            SceneManager.LoadSceneAsync(MainMenuScene);
        }

        public void Dispose() => 
            _iapService.Dispose();
    }
}
using _Project.Scripts.Services.Factory.RemoteConfigFactory;
using _Project.Scripts.Services.LoadingScreen;
using _Project.Scripts.Services.RemoteConfig;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.Scripts.Infrastructure.Project
{
    public class ProjectBootstrapper: IInitializable
    {
        private const string MainSceneName = "Main";
        
        private readonly LoadingCurtainService _loadingCurtain;
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly IRemoteConfigFactory _remoteConfigFactory;
        
        public ProjectBootstrapper(LoadingCurtainService loadingCurtain, IRemoteConfigService remoteConfigService, IRemoteConfigFactory remoteConfigFactory)
        {
            _loadingCurtain = loadingCurtain;
            _remoteConfigService = remoteConfigService;
            _remoteConfigFactory = remoteConfigFactory;
        }

        public async void Initialize()
        {
            await _loadingCurtain.ShowLoading();
            await _remoteConfigService.FetchDataAsync();
            _remoteConfigFactory.ApplyRemoteConfigs();
            SceneManager.LoadSceneAsync(MainSceneName);
        }
    }
}
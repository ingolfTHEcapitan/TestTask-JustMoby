using _Project.Scripts.Services.Factory.RemoteConfigFactory;
using _Project.Scripts.Services.LoadingScreen;
using _Project.Scripts.Services.RemoteConfig;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.Scripts.Infrastructure.Project
{
    public class ProjectBootstrapper: IInitializable
    {
        private const string Main = "Main";
        
        private readonly LoadingScreenService _loadingScreen;
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly IRemoteConfigFactory _remoteConfigFactory;
        
        public ProjectBootstrapper(LoadingScreenService loadingScreen, IRemoteConfigService remoteConfigService, IRemoteConfigFactory remoteConfigFactory)
        {
            _loadingScreen = loadingScreen;
            _remoteConfigService = remoteConfigService;
            _remoteConfigFactory = remoteConfigFactory;
        }

        public async void Initialize()
        {
            await _loadingScreen.ShowLoading();
            await _remoteConfigService.FetchDataAsync();
            _remoteConfigFactory.ApplyRemoteSettings();
            SceneManager.LoadScene(Main);
        }
    }
}
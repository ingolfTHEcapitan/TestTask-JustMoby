using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Services.Ads;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Factory.LoadingCurtainFactory;
using _Project.Scripts.Services.Factory.RemoteConfigFactory;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.LoadingScreen;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.RemoteConfig;
using _Project.Scripts.Services.Statistics;
using Zenject;

namespace _Project.Scripts.Infrastructure.Project
{
    public class ProjectInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            BindRemoteConfig();
            BindServices();
            BindLoadingCurtain();
            BindProjectBootstrapper();
        }

        private void BindRemoteConfig()
        {
            Container.BindInterfacesAndSelfTo<RemoteConfigService>().AsSingle();
            Container.BindInterfacesAndSelfTo<RemoteConfigFactory>().AsSingle();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<AssetProvider>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AdsService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<DesktopInputService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GamePauseService>().AsSingle();
            Container.BindInterfacesAndSelfTo<FirebaseAnalyticsService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStatistics>().AsSingle();
        }

        private void BindLoadingCurtain()
        {
            Container.BindInterfacesAndSelfTo<LoadingCurtainFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<LoadingCurtainService>().AsSingle();
        }
        
        private void BindProjectBootstrapper() => 
            Container.BindInterfacesAndSelfTo<ProjectBootstrapper>().AsSingle();
    }
}
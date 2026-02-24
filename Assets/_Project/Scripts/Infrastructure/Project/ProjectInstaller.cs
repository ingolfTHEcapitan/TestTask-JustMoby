using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Services.Ads;
using _Project.Scripts.Services.Factory.LoadingCurtainFactory;
using _Project.Scripts.Services.Factory.RemoteConfigFactory;
using _Project.Scripts.Services.Factory.UIFactory;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.LoadingScreen;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.RemoteConfig;
using Zenject;

namespace _Project.Scripts.Infrastructure.Project
{
    public class ProjectInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            BindRemoteConfig();
            BindAssetProvider();
            BindLoadingCurtainFactory();
            BindLoadingScreenService();
            BindProjectBootstrapper();
        }

        private void BindRemoteConfig()
        {
            Container.BindInterfacesAndSelfTo<RemoteConfigService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<RemoteConfigFactory>().AsSingle();
        }

        private void BindAssetProvider()
        {
            Container.BindInterfacesAndSelfTo<AssetProvider>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<DesktopInputService>().AsSingle();
            Container.BindInterfacesAndSelfTo<AdsService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GamePauseService>().AsSingle();
        }

        private void BindLoadingCurtainFactory() => 
            Container.BindInterfacesAndSelfTo<LoadingCurtainFactory>().AsSingle().NonLazy();
            

        private void BindLoadingScreenService()
        {
            Container.BindInterfacesAndSelfTo<LoadingScreenService>().AsSingle();
        }

        private void BindProjectBootstrapper() => 
            Container.BindInterfacesAndSelfTo<ProjectBootstrapper>().AsSingle().NonLazy();
    }
}
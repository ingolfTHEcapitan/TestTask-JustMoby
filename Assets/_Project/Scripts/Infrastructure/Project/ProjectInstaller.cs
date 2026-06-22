using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Services.Ads;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Authentication;
using _Project.Scripts.Services.Effects;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.LoadingCurtain.Factory;
using _Project.Scripts.Services.NetworkAccessibility;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.RemoteConfig;
using _Project.Scripts.Services.RemoteConfig.RemoteConfigFactory;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.Services.SaveLoad.CloudSave;
using _Project.Scripts.Services.Statistics;
using _Project.Scripts.UI.Factory;
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
            Container.Bind<PlayerSpawnerConfig>().AsSingle();
            Container.Bind<EnemySpawnerConfig>().AsSingle();
            Container.Bind<WeaponConfig>().AsSingle();
            Container.Bind<BulletConfig>().AsSingle();
            Container.Bind<EnemyConfig>().AsSingle();
            Container.Bind<LocalSaveServiceConfig>().AsSingle();
            Container.Bind<ProductConfigWrapper>().AsSingle();
            Container.Bind<List<PlayerStatConfig>>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<RemoteConfigService>().AsSingle();
            Container.BindInterfacesAndSelfTo<RemoteConfigFactory>().AsSingle();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<NetworkAccessibilityService>().AsSingle();
            Container.BindInterfacesAndSelfTo<AuthService>().AsSingle();
            Container.BindInterfacesAndSelfTo<ProgressService>().AsSingle();
            Container.BindInterfacesAndSelfTo<AssetProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<AdsService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<DesktopInputService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GamePauseService>().AsSingle();
            Container.BindInterfacesAndSelfTo<FirebaseAnalyticsService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStatistics>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIFactory>().AsSingle();

            Container.Bind<ISaveLoadService>().WithId(SaveType.Local).FromMethod(GetLocalSaveInstance).AsCached();
            Container.Bind<ISaveLoadService>().WithId(SaveType.Cloud).To<CloudSaveLoadService>().AsCached();
            Container.Bind<SaveLoadCoordinator>().AsCached();
            Container.Bind<ISaveLoadService>().WithId(SaveType.Coordinator).To<SaveLoadCoordinator>().FromResolve();
            Container.Bind<ISaveLoadCoordinator>().To<SaveLoadCoordinator>().FromResolve();
            
            Container.Bind<IIAPService>().To<IAPService>().AsSingle();
            Container.BindInterfacesAndSelfTo<EffectsService>().AsSingle();
        }

        private void BindLoadingCurtain()
        {
            Container.BindInterfacesAndSelfTo<LoadingCurtainFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<LoadingCurtainService>().AsSingle();
        }
        
        private void BindProjectBootstrapper() => 
            Container.BindInterfacesAndSelfTo<ProjectBootstrapper>().AsSingle();

        private ISaveLoadService GetLocalSaveInstance(InjectContext context) => 
            context.Container.Resolve<LocalSaveServiceConfig>().GetInstance();
    }
}
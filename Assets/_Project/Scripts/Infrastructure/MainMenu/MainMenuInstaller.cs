using _Project.Scripts.UI.Factory;
using _Project.Scripts.UI.Windows.SaveConflictResolve;
using _Project.Scripts.UI.Windows.Settings;
using _Project.Scripts.UI.Windows.Shop;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.MainMenu
{
    public class MainMenuInstaller: MonoInstaller
    {
        [SerializeField] private Transform _uiParent;
        
        public override void InstallBindings()
        {
            BindMainMenuBootstrapper();
            BindSaveConflictResolver();
            BindShopWindow();
            BindSettingsWindow();
        }

        private void BindShopWindow()
        {
            Container.BindInterfacesAndSelfTo<ShopWindowView>().FromMethod(GetShopWindowView).AsSingle();
            Container.BindInterfacesAndSelfTo<ShopWindowModel>().AsSingle();
            Container.Bind<ShopWindowPresenter>().AsSingle();
            Container.Bind<ShopItemFactory>().AsSingle();
        }

        private void BindMainMenuBootstrapper() => 
            Container.BindInterfacesAndSelfTo<MainMenuBootstrapper>().AsSingle().WithArguments(_uiParent);

        private void BindSaveConflictResolver() => 
            Container.BindInterfacesAndSelfTo<SaveConflictResolveService>().AsSingle().WithArguments(_uiParent);

        private void BindSettingsWindow()
        {
            Container.BindInterfacesAndSelfTo<SettingsWindowView>().FromMethod(GetSettingsWindowView).AsSingle();
            Container.BindInterfacesAndSelfTo<SettingsWindowModel>().AsSingle();
            Container.Bind<SettingsWindowPresenter>().AsSingle();
        }

        private SettingsWindowView GetSettingsWindowView(InjectContext context) =>
            context.Container.Resolve<IUIFactory>().GetSettingsWindowView();

        private ShopWindowView GetShopWindowView(InjectContext context) => 
            context.Container.Resolve<IUIFactory>().GetShopWindowView();
    }
}
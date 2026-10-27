using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows.MainMenu;
using _Project.Scripts.UI.Windows.SaveConflictResolve;
using _Project.Scripts.UI.Windows.Settings;
using _Project.Scripts.UI.Windows.Shop;
using _Project.Scripts.UI.Windows.Shop.ShopItem;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.MainMenu
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private Transform _uiParent;
        
        public override void InstallBindings()
        {
            BindSaveConflictResolver();
            BindShopWindow();
            BindSettingsWindow();
            BindMainMenuWindow();
            BindMainMenuBootstrapper();
        }

        private void BindSaveConflictResolver()
        {
            Container.Bind<SaveTimeFormatter>().AsSingle();
            Container.BindWindow<SaveConflictResolveWindowView>();
            Container.Bind<SaveConflictResolveWindowModel>().AsSingle();
            Container.Bind<SaveConflictResolveWindowPresenter>().AsSingle();
        }

        private void BindShopWindow()
        {
            Container.BindWindow<ShopWindowView>();
            Container.BindInterfacesAndSelfTo<ShopWindowModel>().AsSingle();
            Container.Bind<ShopWindowPresenter>().AsSingle();
            Container.Bind<ShopItemFactory>().AsSingle();
        }

        private void BindSettingsWindow()
        {
            Container.BindWindow<SettingsWindowView>();
            Container.BindInterfacesAndSelfTo<SettingsWindowModel>().AsSingle();
            Container.Bind<SettingsWindowPresenter>().AsSingle();
        }

        private void BindMainMenuWindow()
        {
            Container.BindWindow<MainMenuWindowView>();
            Container.BindInterfacesAndSelfTo<MainMenuWindowModel>().AsSingle();
            Container.Bind<MainMenuWindowPresenter>().AsSingle();
        }

        private void BindMainMenuBootstrapper() => 
            Container.BindInterfacesAndSelfTo<MainMenuBootstrapper>().AsSingle().WithArguments(_uiParent);
        
    }
}
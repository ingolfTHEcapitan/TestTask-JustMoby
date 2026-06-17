using _Project.Scripts.Services.SaveConflictResolve;
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
            BindMSaveConflictResolver();
        }

        private void BindMainMenuBootstrapper() => 
            Container.BindInterfacesAndSelfTo<MainMenuBootstrapper>().AsSingle().WithArguments(_uiParent);

        private void BindMSaveConflictResolver() => 
            Container.BindInterfacesAndSelfTo<SaveConflictResolveService>().AsSingle();
    }
}
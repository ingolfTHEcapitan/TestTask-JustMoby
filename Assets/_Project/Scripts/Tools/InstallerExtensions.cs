using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Tools
{
    public static class InstallerExtensions
    {
        public static void BindWindow<TIWindow>(this DiContainer container) where TIWindow : Component, IWindow
        {
            container.BindInterfacesAndSelfTo<TIWindow>()
                .FromMethod(context => context.Container.Resolve<WindowFactory>().Get<TIWindow>()).AsSingle();
        }
    }
}
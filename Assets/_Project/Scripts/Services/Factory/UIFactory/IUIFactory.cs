using System.Threading.Tasks;
using _Project.Scripts.UI.Windows;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.UIFactory
{
    public interface IUIFactory
    {
        Task<GameObject> CreateHudLayer();
        Task<GameObject> CreatePopUpLayer();
        Task<LoadingCurtain> CreateLoadingCurtain();
    }
}
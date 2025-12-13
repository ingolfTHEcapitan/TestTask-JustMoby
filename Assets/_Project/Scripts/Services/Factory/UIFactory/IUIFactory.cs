using System.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.UIFactory
{
    public interface IUIFactory
    {
        Task<GameObject> CreateHudLayer();
        Task<GameObject> CreatePopUpLayer();
    }
}
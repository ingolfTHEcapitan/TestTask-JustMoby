using System.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.UIFactory
{
    public interface IUIFactory
    {
        Task<GameObject> CreateHudLayer(Transform uiParent);
        Task<GameObject> CreatePopUpLayer(Transform uiParent);
        Task<Sprite> LoadSprite(string assetAddress);
    }
}
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.UIFactory
{
    public interface IUIFactory
    {
        UniTask<GameObject> CreateHudLayer(Transform uiParent);
        UniTask<GameObject> CreatePopUpLayer(Transform uiParent);
        UniTask<Sprite> LoadSprite(string assetAddress);
    }
}
using _Project.Scripts.Services.SaveConflictResolve.UI;
using _Project.Scripts.UI.Windows.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Factory
{
    public interface IUIFactory
    {
        UniTask<GameObject> CreateHudLayer(Transform uiParent);
        UniTask<GameObject> CreatePopUpLayer(Transform uiParent);
        UniTask<GameObject> CreateMainMenuLayer(Transform uiParent);
        UniTask<ShopItem> CreateShopItem(Transform parent);
        UniTask<Sprite> LoadSprite(string assetAddress);
        UniTask<SaveConflictResolveWindow> CreateSaveConflictResolveWindow();
    }
}
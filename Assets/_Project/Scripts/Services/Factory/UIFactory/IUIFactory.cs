using System.Threading.Tasks;
using _Project.Scripts.Data;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Windows.SaveConflictResolve;
using _Project.Scripts.UI.Windows.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.UIFactory
{
    public interface IUIFactory
    {
        UniTask<GameObject> CreateHudLayer(Transform uiParent);
        UniTask<GameObject> CreatePopUpLayer(Transform uiParent);
        UniTask<GameObject> CreateMainMenuLayer(Transform uiParent);
        UniTask<ShopItem> CreateShopItem(Transform parent);
        UniTask<Sprite> LoadSprite(string assetAddress);
        Task<SaveConflictResolveWindow> CreateSaveConflictResolveWindow(PlayerProgress localProgress, PlayerProgress cloudProgress);
    }
}
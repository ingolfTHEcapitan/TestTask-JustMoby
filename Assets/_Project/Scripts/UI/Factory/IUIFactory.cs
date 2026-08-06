using _Project.Scripts.Logic.Player.PlayerStats.UI;
using _Project.Scripts.Services.SaveConflictResolve.UI;
using _Project.Scripts.UI.Windows.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Factory
{
    public interface IUIFactory
    {
        UniTask<HeadUpDisplay> CreateHudLayerAsync(Transform uiParent);
        UniTask<GameObject> CreatePopUpLayerAsync(Transform uiParent);
        UniTask<GameObject> CreateMainMenuLayerAsync(Transform uiParent);
        UniTask<ShopItem> CreateShopItemAsync(Transform uiParent);
        UniTask<Sprite> LoadSpriteAsync(string assetAddress);
        UniTask<SaveConflictResolveWindow> CreateSaveConflictResolveWindowAsync(Transform uiParent);
        UniTask<PlayerStatItemView> CreatePlayerStatItemAsync(Transform uiParent);
    }
}
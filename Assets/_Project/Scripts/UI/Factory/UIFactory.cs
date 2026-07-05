using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.Player.PlayerStats.UI;
using _Project.Scripts.Services.SaveConflictResolve.UI;
using _Project.Scripts.UI.Windows.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Factory
{
    public class UIFactory : IUIFactory
    {
        private readonly IInstantiator _container;
        private readonly IAssetProvider _assetProvider;

        public UIFactory(IInstantiator container, IAssetProvider assetProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
        }
        
        public async UniTask<HeadUpDisplay> CreateHudLayer(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.HudLayer);
            return _container.InstantiatePrefabForComponent<HeadUpDisplay>(prefab, uiParent);
        }

        public async UniTask<GameObject> CreatePopUpLayer(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.PopUpLayer);
            return _container.InstantiatePrefab(prefab, uiParent);
        }

        public async UniTask<GameObject> CreateMainMenuLayer(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.MainMenuLayer);
            return _container.InstantiatePrefab(prefab, uiParent);
        }

        public async UniTask<ShopItem> CreateShopItem(Transform parent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.ShopItem);
            return _container.InstantiatePrefabForComponent<ShopItem>(prefab, parent);
        }
        
        public async UniTask<Sprite> LoadSprite(string assetAddress) => 
            await _assetProvider.LoadAsync<Sprite>(assetAddress);

        public async UniTask<SaveConflictResolveWindow> CreateSaveConflictResolveWindow()
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.SaveConflictResolveWindow);
            return _container.InstantiatePrefabForComponent<SaveConflictResolveWindow>(prefab);
        }

        public async UniTask<PlayerStatItemView> CreatePlayerStatItem(Transform parent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.PlayerStatItem);
            return _container.InstantiatePrefabForComponent<PlayerStatItemView>(prefab, parent);
        }
    }
}
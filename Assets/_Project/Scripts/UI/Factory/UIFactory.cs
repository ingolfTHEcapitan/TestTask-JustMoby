using System.Threading.Tasks;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.UI.Windows.SaveConflictResolve;
using _Project.Scripts.UI.Windows.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Factory
{
    public class UIFactory : IUIFactory
    {
        private readonly DiContainer _container;
        private readonly IAssetProvider _assetProvider;

        public UIFactory(DiContainer container, IAssetProvider assetProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
        }
        
        public async UniTask<GameObject> CreateHudLayer(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.HudLayer);
            return _container.InstantiatePrefab(prefab, uiParent);
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

        public async Task<SaveConflictResolveWindow> CreateSaveConflictResolveWindow(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.SaveConflictResolveWindow);
            SaveConflictResolveWindow window = _container.InstantiatePrefabForComponent<SaveConflictResolveWindow>(prefab);
            window.Construct(localProgress, cloudProgress);
            return window;
        }
    }
}
using System.Data;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.UI.HUD;
using _Project.Scripts.UI.Windows.PlayerStats;
using _Project.Scripts.UI.Windows.SaveConflictResolve;
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
        private HeadUpDisplayView _hudView;

        public UIFactory(IInstantiator container, IAssetProvider assetProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
        }
        
        public async UniTask<HeadUpDisplayView> CreateHudLayerAsync(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.HudLayer);
            _hudView = _container.InstantiatePrefabForComponent<HeadUpDisplayView>(prefab, uiParent);
            return _hudView;
        }

        public async UniTask<GameObject> CreatePopUpLayerAsync(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.PopUpLayer);
            return _container.InstantiatePrefab(prefab, uiParent);
        }

        public async UniTask<GameObject> CreateMainMenuLayerAsync(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.MainMenuLayer);
            return _container.InstantiatePrefab(prefab, uiParent);
        }

        public async UniTask<ShopItem> CreateShopItemAsync(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.ShopItem);
            return _container.InstantiatePrefabForComponent<ShopItem>(prefab, uiParent);
        }
        
        public async UniTask<Sprite> LoadSpriteAsync(string assetAddress) => 
            await _assetProvider.LoadAsync<Sprite>(assetAddress);

        public async UniTask<SaveConflictResolveWindow> CreateSaveConflictResolveWindowAsync(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.SaveConflictResolveWindow);
            return _container.InstantiatePrefabForComponent<SaveConflictResolveWindow>(prefab, uiParent);
        }

        public async UniTask<PlayerStatItemView> CreatePlayerStatItemAsync(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.PlayerStatItem);
            return _container.InstantiatePrefabForComponent<PlayerStatItemView>(prefab, uiParent);
        }

        public HeadUpDisplayView GetHudView()
        {
            if (!_hudView)
            {
                throw new InvalidConstraintException
                ("HUD view requested before creation. " +
                 "Ensure HeadUpDisplayPresenter is not resolved before GameUIInitializer ran.");
            }
            
            return _hudView;
        }
    }
}
using System.Data;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Infrastructure.Game;
using _Project.Scripts.UI.HUD;
using _Project.Scripts.UI.Windows.GameOver;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using _Project.Scripts.UI.Windows.MainMenu;
using _Project.Scripts.UI.Windows.PlayerStats;
using _Project.Scripts.UI.Windows.SaveConflictResolve;
using _Project.Scripts.UI.Windows.Settings;
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
        
        public async UniTask<HeadUpDisplayView> CreateHudViewAsync(Transform uiParent)
        {
            _hudView = await CreateViewAsync<HeadUpDisplayView>(uiParent, AssetAddress.HeadUpDisplay);
            return _hudView;
        }

        public async UniTask<GameOverWindow> CreateGameOverWindowAsync(Transform uiParent)=> 
            await CreateViewAsync<GameOverWindow>(uiParent, AssetAddress.GameOverWindow);

        public async UniTask<LoadingCurtain> CreateLoadingCurtainAsync(Transform uiParent)=> 
            await CreateViewAsync<LoadingCurtain>(uiParent, AssetAddress.LoadingCurtain);

        public async UniTask<MainMenuWindow> CreateMainMenuWindowAsync(Transform uiParent)=> 
            await CreateViewAsync<MainMenuWindow>(uiParent, AssetAddress.MainMenuWindow);

        public async UniTask<PlayerStatsView> CreatePlayerStatsViewAsync(Transform uiParent)=> 
            await CreateViewAsync<PlayerStatsView>(uiParent, AssetAddress.PlayerStatsWindow);
        
        public async UniTask<PlayerStatItemView> CreatePlayerStatItemViewAsync(Transform uiParent)=> 
            await CreateViewAsync<PlayerStatItemView>(uiParent, AssetAddress.PlayerStatItem);
        
        public async UniTask<SaveConflictResolveWindow> CreateSaveConflictResolveWindowAsync(Transform uiParent)=> 
            await CreateViewAsync<SaveConflictResolveWindow>(uiParent, AssetAddress.SaveConflictResolveWindow);
        
        public async UniTask<SettingsView> CreateSettingsViewAsync(Transform uiParent)=> 
            await CreateViewAsync<SettingsView>(uiParent, AssetAddress.SettingsWindow);
        
        public async UniTask<ShopWindow> CreateShopWindowAsync(Transform uiParent)=> 
            await CreateViewAsync<ShopWindow>(uiParent, AssetAddress.ShopWindow);
        
        public async UniTask<ShopItem> CreateShopItemAsync(Transform uiParent)=> 
            await CreateViewAsync<ShopItem>(uiParent, AssetAddress.ShopItem);
        
        public async UniTask<Sprite> LoadSpriteAsync(string assetAddress) => 
            await _assetProvider.LoadAsync<Sprite>(assetAddress);

        public HeadUpDisplayView GetView()
        {
            if (_hudView)
                return _hudView;
            
            throw new InvalidConstraintException
            ($"{_hudView.gameObject.name} view requested before creation. " +
             $"Ensure presenter depending on it is not resolved before {nameof(GameUIInitializer)} runs");
        }
        
        private async UniTask<TView> CreateViewAsync<TView>(Transform uiParent, string assetAddress) where TView : MonoBehaviour
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(assetAddress);
            TView view = _container.InstantiatePrefabForComponent<TView>(prefab, uiParent);
            return view;
        }
    }
}
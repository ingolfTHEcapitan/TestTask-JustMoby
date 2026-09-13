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
            _hudView = await CreateViewAsync<HeadUpDisplayView>(AssetAddress.HeadUpDisplay, uiParent);
            return _hudView;
        }

        public async UniTask<GameOverWindow> CreateGameOverWindowAsync(Transform uiParent)=> 
            await CreateViewAsync<GameOverWindow>(AssetAddress.GameOverWindow, uiParent);

        public async UniTask<LoadingCurtain> CreateLoadingCurtainAsync()=> 
            await CreateViewAsync<LoadingCurtain>(AssetAddress.LoadingCurtain);

        public async UniTask<MainMenuWindow> CreateMainMenuWindowAsync(Transform uiParent)=> 
            await CreateViewAsync<MainMenuWindow>(AssetAddress.MainMenuWindow, uiParent);

        public async UniTask<PlayerStatsView> CreatePlayerStatsViewAsync(Transform uiParent)=> 
            await CreateViewAsync<PlayerStatsView>(AssetAddress.PlayerStatsWindow, uiParent);
        
        public async UniTask<PlayerStatItemView> CreatePlayerStatItemViewAsync(Transform uiParent)=> 
            await CreateViewAsync<PlayerStatItemView>(AssetAddress.PlayerStatItem, uiParent);
        
        public async UniTask<SaveConflictResolveWindow> CreateSaveConflictResolveWindowAsync(Transform uiParent)=> 
            await CreateViewAsync<SaveConflictResolveWindow>(AssetAddress.SaveConflictResolveWindow, uiParent);
        
        public async UniTask<SettingsView> CreateSettingsViewAsync(Transform uiParent)=> 
            await CreateViewAsync<SettingsView>(AssetAddress.SettingsWindow, uiParent);
        
        public async UniTask<ShopWindow> CreateShopWindowAsync(Transform uiParent)=> 
            await CreateViewAsync<ShopWindow>(AssetAddress.ShopWindow, uiParent);
        
        public async UniTask<ShopItem> CreateShopItemAsync(Transform uiParent)=> 
            await CreateViewAsync<ShopItem>(AssetAddress.ShopItem, uiParent);
        
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
        
        private async UniTask<TView> CreateViewAsync<TView>(string assetAddress,Transform uiParent = null) where TView : MonoBehaviour
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(assetAddress);
            TView view = _container.InstantiatePrefabForComponent<TView>(prefab, uiParent);
            return view;
        }
    }
}
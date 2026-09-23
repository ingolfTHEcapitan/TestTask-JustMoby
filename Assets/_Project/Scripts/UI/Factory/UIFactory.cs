using System.Data;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Infrastructure.Game;
using _Project.Scripts.Infrastructure.MainMenu;
using _Project.Scripts.Infrastructure.Project;
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
        private LoadingCurtainView _loadingCurtainView;
        private SettingsWindowView _settingsWindowView;
        private GameOverWindowView _gameOverView;
        private ShopWindowView _shopWindowView;
        private PlayerStatsWindowView _playerStatsWindowView;
        private MainMenuWindowView _mainMenuWindowView;

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

        public async UniTask<GameOverWindowView> CreateGameOverWindowViewAsync(Transform uiParent)
        {
            _gameOverView = await CreateViewAsync<GameOverWindowView>(AssetAddress.GameOverWindow, uiParent);
            return _gameOverView;
        }

        public async UniTask<LoadingCurtainView> CreateLoadingCurtainViewAsync()
        {
            _loadingCurtainView = await CreateViewAsync<LoadingCurtainView>(AssetAddress.LoadingCurtain);
            return _loadingCurtainView;
        }

        public async UniTask<MainMenuWindowView> CreateMainMenuWindowViewAsync(Transform uiParent)
        {
            _mainMenuWindowView = await CreateViewAsync<MainMenuWindowView>(AssetAddress.MainMenuWindow, uiParent);
            return _mainMenuWindowView;
        }

        public async UniTask<PlayerStatsWindowView> CreatePlayerStatsViewAsync(Transform uiParent)
        {
            _playerStatsWindowView = await CreateViewAsync<PlayerStatsWindowView>(AssetAddress.PlayerStatsWindow, uiParent);
            return _playerStatsWindowView;
        }

        public async UniTask<PlayerStatItemView> CreatePlayerStatItemViewAsync(Transform uiParent)=> 
            await CreateViewAsync<PlayerStatItemView>(AssetAddress.PlayerStatItem, uiParent);
        
        public async UniTask<SaveConflictResolveWindow> CreateSaveConflictResolveWindowAsync(Transform uiParent)=> 
            await CreateViewAsync<SaveConflictResolveWindow>(AssetAddress.SaveConflictResolveWindow, uiParent);
        
        public async UniTask<SettingsWindowView> CreateSettingsViewAsync(Transform uiParent)
        {
            _settingsWindowView = await CreateViewAsync<SettingsWindowView>(AssetAddress.SettingsWindow, uiParent);
            return _settingsWindowView;
        }
        
        public async UniTask<ShopWindowView> CreateShopWindowViewAsync(Transform uiParent)
        {
            _shopWindowView = await CreateViewAsync<ShopWindowView>(AssetAddress.ShopWindow, uiParent);
            return _shopWindowView;
        }

        public async UniTask<Sprite> LoadSpriteAsync(string assetAddress) => 
            await _assetProvider.LoadAsync<Sprite>(assetAddress);

        public HeadUpDisplayView GetHudView()
        {
            if (_hudView)
                return _hudView;
            
            throw new InvalidConstraintException
            ($"{_hudView.gameObject.name} view requested before creation. " +
             $"Ensure presenter depending on it is not resolved before {nameof(GameUIInitializer)} runs");
        }
        
        public LoadingCurtainView GetLoadingWindowView()
        {
            if (_loadingCurtainView)
                return _loadingCurtainView;
            
            throw new InvalidConstraintException
            ($"{_loadingCurtainView.gameObject.name} view requested before creation. " +
             $"Ensure presenter depending on it is not resolved before {nameof(ProjectBootstrapper)} runs");
        }
        
        public SettingsWindowView GetSettingsWindowView()
        {
            if (_settingsWindowView)
                return _settingsWindowView;
            
            throw new InvalidConstraintException
            ($"{_settingsWindowView.gameObject.name} view requested before creation. " +
             $"Ensure presenter depending on it is not resolved before {nameof(MainMenuBootstrapper)} runs");
        }

        public PlayerStatsWindowView GetPlayerStatsWindowView()
        {
            if (_playerStatsWindowView)
                return _playerStatsWindowView;
            
            throw new InvalidConstraintException
            ($"{_playerStatsWindowView.gameObject.name} view requested before creation. " +
             $"Ensure presenter depending on it is not resolved before {nameof(GameUIInitializer)} runs");
        }

        public ShopWindowView GetShopWindowView()
        {
            if (_shopWindowView)
                return _shopWindowView;
            
            throw new InvalidConstraintException
            ($"{_shopWindowView.gameObject.name} view requested before creation. " +
             $"Ensure presenter depending on it is not resolved before {nameof(MainMenuBootstrapper)} runs");
        }

        public MainMenuWindowView GetMainMenuWindowView()
        {
            if (_mainMenuWindowView)
                return _mainMenuWindowView;
            
            throw new InvalidConstraintException
            ($"{_mainMenuWindowView.gameObject.name} view requested before creation. " +
             $"Ensure presenter depending on it is not resolved before {nameof(MainMenuBootstrapper)} runs");
        }

        public GameOverWindowView GetGameOverWindowView()
        {
            if (_gameOverView)
                return _gameOverView;
            
            throw new InvalidConstraintException
            ($"{_gameOverView.gameObject.name} view requested before creation. " +
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
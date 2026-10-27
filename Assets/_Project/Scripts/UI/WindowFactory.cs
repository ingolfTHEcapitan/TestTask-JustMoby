using System;
using System.Collections.Generic;
using System.Data;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.UI.HUD;
using _Project.Scripts.UI.Windows;
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

namespace _Project.Scripts.UI
{
    public class WindowFactory
    {
        private readonly IInstantiator _container;
        private readonly IAssetProvider _assetProvider;
        
        private readonly Dictionary<Type, IWindow> _windowViews = new Dictionary<Type, IWindow>();
        private readonly Dictionary<Type, string> _viewAddresses = new Dictionary<Type, string>()
        {
            {typeof(HeadUpDisplayView), AssetAddress.HeadUpDisplay},
            {typeof(GameOverWindowView), AssetAddress.GameOverWindow},
            {typeof(LoadingCurtainView), AssetAddress.LoadingCurtain},
            {typeof(MainMenuWindowView), AssetAddress.MainMenuWindow},
            {typeof(PlayerStatsWindowView), AssetAddress.PlayerStatsWindow},
            {typeof(SaveConflictResolveWindowView), AssetAddress.SaveConflictResolveWindow},
            {typeof(SettingsWindowView), AssetAddress.SettingsWindow},
            {typeof(ShopWindowView), AssetAddress.ShopWindow},
        };
        
        public WindowFactory(IInstantiator container, IAssetProvider assetProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
        }
        
        public async UniTask<TWindow> CreateAsync<TWindow>(Transform parent = null, bool isGlobal = false) where TWindow : Component, IWindow
        {
            Type viewType = typeof(TWindow);
            
            if (_windowViews.TryGetValue(viewType, out IWindow windowView))
                return (TWindow)windowView;
            
            if (!_viewAddresses.TryGetValue(viewType, out string assetAddress))
                throw new KeyNotFoundException("[WindowFactory] No asset address registered for window : " + viewType.Name);

            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(assetAddress, isGlobal);
            TWindow view = _container.InstantiatePrefabForComponent<TWindow>(prefab, parent);
            
            _windowViews[viewType] = view;
            view.OnWindowDestroy += DestroyView;

            return view;

            void DestroyView()
            {
                view.OnWindowDestroy -= DestroyView;
                _windowViews.Remove(viewType);
            }
        }
        
        public TWindow Get<TWindow>() where TWindow : Component, IWindow
        {
            Type viewType = typeof(TWindow);
            
            if (_windowViews.TryGetValue(viewType, out IWindow windowView))
                return (TWindow)windowView;
            
            throw new InvalidConstraintException(
                $"[WindowFactory] {viewType.Name} requested before creation. Ensure it created before presenter resolution");
        }
    }
}
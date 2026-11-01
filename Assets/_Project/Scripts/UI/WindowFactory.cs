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
        private readonly WindowsRepository _repository;
        
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
        
        public WindowFactory(IInstantiator container, IAssetProvider assetProvider, WindowsRepository repository)
        {
            _container = container;
            _assetProvider = assetProvider;
            _repository = repository;
        }
        
        public async UniTask<TWindow> CreateAsync<TWindow>(Transform parent = null, bool isGlobal = false) where TWindow : Component, IWindow
        {
            Type viewType = typeof(TWindow);
            
            if (_repository.TryGet(out TWindow window))
                return window;
            
            if (!_viewAddresses.TryGetValue(viewType, out string assetAddress))
                throw new KeyNotFoundException("[WindowFactory] No asset address registered for window : " + viewType.Name);

            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(assetAddress, isGlobal);
            TWindow view = _container.InstantiatePrefabForComponent<TWindow>(prefab, parent);
            
            _repository.Register(view);
            
            view.OnWindowDestroy += DestroyView;

            return view;

            void DestroyView()
            {
                view.OnWindowDestroy -= DestroyView;
                _repository.UnRegister<TWindow>();
            }
        }
    }
}
using _Project.Scripts.Configs;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI
{
    public class WindowFactory
    {
        private IInstantiator _container;
        private readonly IAssetProvider _assetProvider;
        private readonly WindowsRepository _repository;
        private readonly WindowAddressesConfig _config;
        
        public WindowFactory(IInstantiator container, IAssetProvider assetProvider, WindowsRepository repository, WindowAddressesConfig config)
        {
            _container = container;
            _assetProvider = assetProvider;
            _repository = repository;
            _config = config;
        }
        
        public async UniTask<TWindow> CreateAsync<TWindow>(Transform parent = null, bool isGlobal = false, IInstantiator sceneContainer = null) where TWindow : Component, IWindow
        {
            if (sceneContainer != null)
                _container = sceneContainer;
            
            if (_repository.TryGet(out TWindow window))
                return window;
            
            string assetAddress = _config.GetAssetAddress<TWindow>();

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
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.AssetManagement;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.Factory.UIFactory
{
    public class UIFactory : IUIFactory
    {
        private readonly DiContainer _container;
        private readonly Transform _uiParent;
        private IAssetProvider _assetProvider;

        public UIFactory(DiContainer container, Transform uiParent, IAssetProvider assetProvider)
        {
            _container = container;
            _uiParent = uiParent;
            _assetProvider = assetProvider;
        }
        
        public async Task<GameObject> CreateHudLayer()
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.HudLayer);
            return _container.InstantiatePrefab(prefab, _uiParent);
        }

        public async Task<GameObject> CreatePopUpLayer()
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.PopUpLayer);
            return _container.InstantiatePrefab(prefab, _uiParent);
        }
    }
}
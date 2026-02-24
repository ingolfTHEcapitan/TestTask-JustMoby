using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.Factory.UIFactory
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
        
        public async Task<GameObject> CreateHudLayer(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.HudLayer);
            return _container.InstantiatePrefab(prefab, uiParent);
        }

        public async Task<GameObject> CreatePopUpLayer(Transform uiParent)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.PopUpLayer);
            return _container.InstantiatePrefab(prefab, uiParent);
        }
    }
}
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.Factory.LoadingCurtainFactory
{
    public class LoadingCurtainFactory : ILoadingCurtainFactory
    {
        private readonly DiContainer _container;
        private readonly IAssetProvider _assetProvider;

        public LoadingCurtainFactory(DiContainer container, IAssetProvider assetProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
        }
        
        
        public async Task<LoadingCurtain> CreateLoadingCurtain()
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.LoadingCurtain);
            return _container.InstantiatePrefabForComponent<UI.Windows.LoadingCurtain>(prefab);
        }
    }
}
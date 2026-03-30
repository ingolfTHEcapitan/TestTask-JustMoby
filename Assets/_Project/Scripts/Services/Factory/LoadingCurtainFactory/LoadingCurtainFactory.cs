using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
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
        
        public async UniTask<LoadingCurtain> CreateLoadingCurtain()
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.LoadingCurtain);
            return _container.InstantiatePrefabForComponent<LoadingCurtain>(prefab);
        }
    }
}
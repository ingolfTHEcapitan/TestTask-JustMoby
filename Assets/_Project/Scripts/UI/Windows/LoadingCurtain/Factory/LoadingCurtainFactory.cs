using _Project.Scripts.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Windows.LoadingCurtain.Factory
{
    public class LoadingCurtainFactory : ILoadingCurtainFactory
    {
        private readonly IInstantiator _container;
        private readonly IAssetProvider _assetProvider;

        public LoadingCurtainFactory(IInstantiator container, IAssetProvider assetProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
        }
        
        public async UniTask<LoadingCurtain> CreateLoadingCurtainAsync()
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.LoadingCurtain);
            return _container.InstantiatePrefabForComponent<LoadingCurtain>(prefab);
        }
    }
}
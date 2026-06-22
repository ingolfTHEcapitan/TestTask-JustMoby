using _Project.Scripts.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.Effects
{
    public class EffectsService : IEffectsService
    {
        private readonly IInstantiator _container;
        private readonly IAssetProvider _assetProvider;

        public EffectsService(IInstantiator container, IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
            _container = container;
        }

        public async UniTask WarmUp()
        {
            await _assetProvider.LoadAsync<GameObject>(AssetAddress.HitFx);
            await _assetProvider.LoadAsync<GameObject>(AssetAddress.Deathfx);
        }

        public async UniTask PlayHitFx(Vector3 position, Transform parent)
        {
            Quaternion rotation = Quaternion.Euler(0, 90, 0);
            
            GameObject fxPrefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.HitFx);
            GameObject fxGameObject = _container.InstantiatePrefab(fxPrefab, position , rotation, parent);
            fxGameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }
        
        public async UniTask PlayEnemyDeathFx(Vector3 position, Transform parent)
        {
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            
            GameObject fxPrefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.Deathfx);
            GameObject fxGameObject = _container.InstantiatePrefab(fxPrefab, position , rotation, parent);
            fxGameObject.transform.localPosition = new Vector3(0, 0.5f, -1);
        }
    }
}
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.Player.PlayerStats;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.Weapon.Bullet.Factory
{
    public class BulletFactory : IBulletFactory
    {
        private readonly IInstantiator _container;
        private readonly PlayerStatsData _playerStatsData;
        private readonly IAssetProvider _assetProvider;
        private readonly Transform _dynamicObjectsParent;

        public BulletFactory(IInstantiator container, PlayerStatsData playerStatsData, 
            IAssetProvider assetProvider, Transform dynamicObjectsParent)
        {
            _container = container;
            _playerStatsData = playerStatsData;
            _assetProvider = assetProvider;
            _dynamicObjectsParent = dynamicObjectsParent;
        }

        public async UniTask<Bullet> CreateBulletAsync(BulletConfig config, Transform at, Vector3 shootDirection, Vector3 targetPoint)
        {
            float damage = _playerStatsData.GetStatValue(StatName.Damage);
            
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.Bullet);
            Bullet bullet = _container.InstantiatePrefab(prefab, at).GetComponent<Bullet>();
            bullet.Initialize(config, shootDirection, targetPoint, damage, _dynamicObjectsParent);
            return bullet;
        }
    }
}
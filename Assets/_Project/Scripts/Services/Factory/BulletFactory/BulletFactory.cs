using System.Threading.Tasks;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Weapon;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.Factory.BulletFactory
{
    public class BulletFactory : IBulletFactory
    {
        private readonly DiContainer _container;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly IAssetProvider _assetProvider;
        private readonly Transform _dynamicObjectsParent;

        public BulletFactory(DiContainer container, PlayerStatsModel playerStatsModel, 
            IAssetProvider assetProvider, Transform dynamicObjectsParent)
        {
            _container = container;
            _playerStatsModel = playerStatsModel;
            _assetProvider = assetProvider;
            _dynamicObjectsParent = dynamicObjectsParent;
        }

        public async Task<Bullet> CreateBullet(BulletConfig config, Transform at, Vector3 shootDirection)
        {
            float damage = _playerStatsModel.GetStatValue(StatName.Damage);
            
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(config.PrefabReference);
            Bullet bullet = _container.InstantiatePrefab(prefab, at).GetComponent<Bullet>();
            bullet.Initialize(config, shootDirection, damage, _dynamicObjectsParent);
            return bullet;
        }
    }
}
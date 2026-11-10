using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.PlayerStats;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.Weapon.Bullet
{
    public class BulletFactory
    {
        private readonly IInstantiator _container;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly IAssetProvider _assetProvider;
        private readonly Transform _bulletContainer;
        private readonly BulletConfig _config;
        
        private CustomPool<Bullet> pool;
        private GameObject _prefab;

        public BulletFactory(IInstantiator container, PlayerStatsModel playerStatsModel, IAssetProvider assetProvider, 
            Transform bulletContainer, BulletConfig config)
        {
            _container = container;
            _playerStatsModel = playerStatsModel;
            _assetProvider = assetProvider;
            _bulletContainer = bulletContainer;
            _config = config;
        }

        public async UniTask Initialize()
        {
            _prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.Bullet);
            pool = new CustomPool<Bullet>(_config.PoolSize, InstantiateBullet, isAutoExpand: true);
        }

        public Bullet CreateBullet(Transform at, Vector3 shootDirection, Vector3 targetPoint)
        {
            float damage = _playerStatsModel.GetStatValue(StatName.Damage);
            Bullet bullet = pool.GetFreeElement();
            bullet.Initialize(_config, at, shootDirection, targetPoint, damage, _bulletContainer, pool.ReturnToPool);
            return bullet;
        }
        
        private Bullet InstantiateBullet() => 
            _container.InstantiatePrefabForComponent<Bullet>(_prefab, _bulletContainer);
    }
}
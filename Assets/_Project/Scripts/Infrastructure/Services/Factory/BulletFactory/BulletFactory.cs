using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Weapon;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Services.Factory.BulletFactory
{
    public class BulletFactory : IBulletFactory
    {
        private readonly DiContainer _container;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly Transform _dynamicObjectsParent;

        public BulletFactory(DiContainer container, PlayerStatsModel playerStatsModel, Transform dynamicObjectsParent)
        {
            _container = container;
            _playerStatsModel = playerStatsModel;
            _dynamicObjectsParent = dynamicObjectsParent;
        }

        public Bullet CreateBullet(BulletConfig config, Transform at, Vector3 shootDirection)
        {
            float damage = _playerStatsModel.GetStatValue(StatName.Damage);
            
            Bullet bullet = _container.InstantiatePrefab(config.Prefab, at).GetComponent<Bullet>();
            bullet.Initialize(config, shootDirection, damage, _dynamicObjectsParent);
            return bullet;
        }
    }
}
using _Project._Scripts.Configs;
using _Project._Scripts.Enemy;
using _Project._Scripts.Infrastructure.Services.AssetManagement;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Logic.PlayerStats;
using _Project._Scripts.Logic.Weapon;
using _Project._Scripts.UI.Elements;
using UnityEngine;

namespace _Project._Scripts.Infrastructure.Services.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IGamePauseService _pauseService;
        private readonly IAssetProvider _assets;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly Transform _dynamicObjectsParent;

        public GameFactory(IAssetProvider assets, IGamePauseService pauseService, PlayerStatsModel playerStatsModel,
            Transform dynamicObjectsParent)
        {
            _assets = assets;
            _pauseService = pauseService;
            _playerStatsModel = playerStatsModel;
            _dynamicObjectsParent = dynamicObjectsParent;
        }

        public GameObject CreateEnemy(EnemySpawnerConfig config, Vector3 at)
        {
            GameObject enemy = Object.Instantiate(config.Prefab, at, Quaternion.identity, _dynamicObjectsParent);
            
            enemy.GetComponent<EnemyAgent>().Construct(_pauseService);
            enemy.GetComponent<EnemyDeath>().Construct(_playerStatsModel);;
            
            HealthBarView healthBar = enemy.GetComponentInChildren<HealthBarView>();
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            healthBar.Construct(health);
            healthBar.Initialize();
            return enemy;
        }

        public Bullet CreateBullet(Vector3 at, Vector3 shootDirection)
        {
            float damage = _playerStatsModel.GetStatValue(StatName.Damage);
            
            Bullet bullet = _assets.Instantiate(AssetPath.Bullet, at).GetComponent<Bullet>();
            bullet.Initialize(shootDirection, damage, _dynamicObjectsParent);
            return bullet;
        }
    }
}
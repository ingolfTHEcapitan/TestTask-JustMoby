using _Project._Scripts.Configs;
using _Project._Scripts.Configs.Weapon;
using _Project._Scripts.Enemy;
using _Project._Scripts.Infrastructure.Services.HealthCalculator;
using _Project._Scripts.Logic;
using _Project._Scripts.Logic.PlayerStats;
using _Project._Scripts.Logic.Weapon;
using _Project._Scripts.UI.Elements;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace _Project._Scripts.Infrastructure.Services.Factory
{
    [UsedImplicitly]
    public class GameFactory : IGameFactory
    {
        private readonly DiContainer _container;
        private readonly IHealthCalculatorService _healthCalculator;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly Transform _dynamicObjectsParent;
        private readonly Transform _UIParent;
        private readonly Transform _gameParent;

        public GameFactory(DiContainer container, IHealthCalculatorService healthCalculator, PlayerStatsModel playerStatsModel,
            Transform dynamicObjectsParent, Transform uiParent, Transform gameParent)
        {
            _container = container;
            _playerStatsModel = playerStatsModel;
            _healthCalculator = healthCalculator;
            _dynamicObjectsParent = dynamicObjectsParent;
            _UIParent = uiParent;
            _gameParent = gameParent;
        }
        

        public GameObject CreateEnemy(EnemySpawnerConfig config, Vector3 spawnPoint)
        {
            GameObject enemy = _container.InstantiatePrefab(config.Prefab, spawnPoint, Quaternion.identity, _dynamicObjectsParent);
            
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            enemyMovement.Initialize(spawnPoint);
                
            float maxHealth = _healthCalculator.CalculateEnemyMaxHealth();
            Health health = enemy.GetComponent<Health>();
            health.Initialize(maxHealth);
            
            HealthBarView healthBar = enemy.GetComponentInChildren<HealthBarView>();
            healthBar.Construct(health);
            healthBar.Initialize();
            return enemy;
        }

        public GameObject CreatePlayer(GameObject prefab, Vector3 at)
        {
            GameObject player = _container.InstantiatePrefab(prefab, at, Quaternion.identity, _gameParent);
            
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            Health health = player.GetComponent<Health>();
            health.Initialize(maxHealth);
            return player;
        }
        
        public GameObject CreateHud(GameObject prefab) => 
            _container.InstantiatePrefab(prefab, _UIParent);
        
        public GameObject CreatePopUpLayer(GameObject prefab) => 
            _container.InstantiatePrefab(prefab, _UIParent);

        public Bullet CreateBullet(BulletConfig config, Transform at, Vector3 shootDirection)
        {
            float damage = _playerStatsModel.GetStatValue(StatName.Damage);
            
            Bullet bullet = _container.InstantiatePrefab(config.Prefab, at).GetComponent<Bullet>();
            bullet.Initialize(config, shootDirection, damage, _dynamicObjectsParent);
            return bullet;
        }
    }
}
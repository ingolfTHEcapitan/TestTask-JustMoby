using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Enemy;
using _Project.Scripts.Infrastructure.Services.HealthCalculator;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Weapon;
using _Project.Scripts.UI.Elements;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Services.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly DiContainer _container;
        private readonly IHealthCalculatorService _healthCalculator;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly Transform _dynamicObjectsParent;
        private readonly Transform _uiParent;
        private readonly Transform _gameParent;

        public GameFactory(DiContainer container, IHealthCalculatorService healthCalculator, PlayerStatsModel playerStatsModel,
            Transform dynamicObjectsParent, Transform uiParent, Transform gameParent)
        {
            _container = container;
            _playerStatsModel = playerStatsModel;
            _healthCalculator = healthCalculator;
            _dynamicObjectsParent = dynamicObjectsParent;
            _uiParent = uiParent;
            _gameParent = gameParent;
        }
        

        public EnemyDeath CreateEnemy(EnemySpawnerConfig config, Vector3 spawnPoint)
        {
            EnemyDeath enemyDeath = 
                _container.InstantiatePrefabForComponent<EnemyDeath>(config.Prefab, spawnPoint, Quaternion.identity, _dynamicObjectsParent);
            
            EnemyMovement enemyMovement = enemyDeath.GetComponent<EnemyMovement>();
            enemyMovement.Initialize(spawnPoint);
                
            float maxHealth = _healthCalculator.CalculateEnemyMaxHealth();
            Health health = enemyDeath.GetComponent<Health>();
            health.Initialize(maxHealth);
            
            HealthBarView healthBar = enemyDeath.GetComponentInChildren<HealthBarView>();
            healthBar.Construct(health);
            healthBar.Initialize();
            return enemyDeath;
        }

        public Health CreatePlayer(GameObject prefab, Vector3 at)
        {
            Health playerHealth = 
                _container.InstantiatePrefabForComponent<Health>(prefab, at, Quaternion.identity, _gameParent);
            
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            playerHealth.Initialize(maxHealth);
            return playerHealth;
        }

        public Bullet CreateBullet(BulletConfig config, Transform at, Vector3 shootDirection)
        {
            float damage = _playerStatsModel.GetStatValue(StatName.Damage);
            
            Bullet bullet = _container.InstantiatePrefab(config.Prefab, at).GetComponent<Bullet>();
            bullet.Initialize(config, shootDirection, damage, _dynamicObjectsParent);
            return bullet;
        }

        public GameObject CreateHudLayer(GameObject prefab) => 
            _container.InstantiatePrefab(prefab, _uiParent);

        public GameObject CreatePopUpLayer(GameObject prefab) => 
            _container.InstantiatePrefab(prefab, _uiParent);
    }
}
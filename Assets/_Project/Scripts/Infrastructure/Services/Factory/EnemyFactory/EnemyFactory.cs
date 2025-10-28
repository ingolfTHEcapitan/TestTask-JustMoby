using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Enemy;
using _Project.Scripts.Infrastructure.Services.HealthCalculator;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.UI.Elements;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Services.Factory.EnemyFactory
{
    public class EnemyFactory: IEnemyFactory
    {
        private readonly DiContainer _container;
        private readonly Transform _dynamicObjectsParent;
        private readonly IHealthCalculatorService _healthCalculator;

        public EnemyFactory(DiContainer container, IHealthCalculatorService healthCalculator, Transform dynamicObjectsParent)
        {
            _container = container;
            _healthCalculator = healthCalculator;
            _dynamicObjectsParent = dynamicObjectsParent;
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
    }
}


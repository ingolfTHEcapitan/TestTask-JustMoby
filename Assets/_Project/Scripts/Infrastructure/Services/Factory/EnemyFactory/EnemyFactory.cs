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

        public EnemyDeath CreateEnemy(EnemySpawnerConfig config, Vector3 spawnPoint, Transform playerTransform)
        {
            EnemyStateMachine enemyStateMachine = 
                _container.InstantiatePrefabForComponent<EnemyStateMachine>(config.Prefab, spawnPoint, Quaternion.identity, _dynamicObjectsParent);

            EnemyRotateToPlayer enemyRotateToPlayer = enemyStateMachine.GetComponent<EnemyRotateToPlayer>();
            enemyRotateToPlayer.Initialize(playerTransform);
            
            enemyStateMachine.Initialize(spawnPoint, playerTransform, enemyRotateToPlayer);
            
            float maxHealth = _healthCalculator.CalculateEnemyMaxHealth();
            Health health = enemyStateMachine.GetComponent<Health>();
            health.Initialize(maxHealth);
            
            HealthBarView healthBar = enemyStateMachine.GetComponentInChildren<HealthBarView>();
            healthBar.Construct(health);
            healthBar.Initialize();

            EnemyDeath enemyDeath = enemyStateMachine.GetComponent<EnemyDeath>();
            enemyDeath.Initialize();
            return enemyDeath;
        }
    }
}


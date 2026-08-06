using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Enemy.States;
using _Project.Scripts.Services.HealthCalculator;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Enemy.Factory
{
    public class EnemyFactory: IEnemyFactory
    {
        private readonly IInstantiator _container;
        private readonly Transform _dynamicObjectsParent;
        private readonly IHealthCalculatorService _healthCalculator;
        private readonly IAssetProvider _assetProvider;

        public EnemyFactory(IInstantiator container, IHealthCalculatorService healthCalculator, 
            IAssetProvider assetProvider, Transform dynamicObjectsParent)
        {
            _container = container;
            _healthCalculator = healthCalculator;
            _assetProvider = assetProvider;
            _dynamicObjectsParent = dynamicObjectsParent;
        }

        public async UniTask<EnemyDeath> CreateEnemyAsync(Vector3 spawnPoint, Transform playerTransform)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.Enemy);
            
            EnemyStateMachine enemyStateMachine = 
                _container.InstantiatePrefabForComponent<EnemyStateMachine>(prefab, spawnPoint, Quaternion.identity, _dynamicObjectsParent);

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
            
            EnemyAnimator enemyAnimator = enemyStateMachine.GetComponent<EnemyAnimator>();
            enemyAnimator.Construct(enemyStateMachine.GetState<EnemyAttackState>());
            enemyAnimator.Initialize();
            return enemyDeath;
        }
    }
}


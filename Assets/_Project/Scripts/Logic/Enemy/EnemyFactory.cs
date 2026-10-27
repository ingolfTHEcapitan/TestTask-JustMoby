using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.Common.Health;
using _Project.Scripts.Logic.Enemy.States;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Common.HealthBar;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Enemy
{
    public class EnemyFactory
    {
        private readonly IInstantiator _container;
        private readonly Transform _dynamicObjectsParent;
        private readonly HealthCalculator _healthCalculator;
        private readonly IAssetProvider _assetProvider;
        private readonly IGamePauseService _pauseService;

        public EnemyFactory(IInstantiator container, HealthCalculator healthCalculator, 
            IAssetProvider assetProvider, Transform dynamicObjectsParent, IGamePauseService pauseService)
        {
            _container = container;
            _healthCalculator = healthCalculator;
            _assetProvider = assetProvider;
            _dynamicObjectsParent = dynamicObjectsParent;
            _pauseService = pauseService;
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
            enemyAnimator.Construct(enemyStateMachine.GetState<EnemyAttackState>(), _pauseService);
            enemyAnimator.Initialize();
            return enemyDeath;
        }
    }
}


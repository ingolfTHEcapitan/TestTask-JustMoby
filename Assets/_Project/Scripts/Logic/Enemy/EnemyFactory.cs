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
        private readonly HealthBarFactory _healthBarFactory;

        public EnemyFactory(IInstantiator container, HealthCalculator healthCalculator, HealthBarFactory healthBarFactory,
            IAssetProvider assetProvider, Transform dynamicObjectsParent, IGamePauseService pauseService)
        {
            _container = container;
            _healthCalculator = healthCalculator;
            _assetProvider = assetProvider;
            _healthBarFactory = healthBarFactory;
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
            Health healthModel = enemyStateMachine.GetComponent<Health>();
            healthModel.Initialize(maxHealth);
            
            HealthBarView healthBarView = enemyStateMachine.GetComponentInChildren<HealthBarView>();
            HealthBarPresenter healthBarPresenter = _healthBarFactory.Create(healthBarView, healthModel);

            EnemyDeath enemyDeath = enemyStateMachine.GetComponent<EnemyDeath>();
            enemyDeath.Initialize();
            
            EnemyAnimator enemyAnimator = enemyStateMachine.GetComponent<EnemyAnimator>();
            enemyAnimator.Construct(enemyStateMachine.GetState<EnemyAttackState>(), _pauseService, healthBarPresenter);
            enemyAnimator.Initialize();
            return enemyDeath;
        }
    }
}


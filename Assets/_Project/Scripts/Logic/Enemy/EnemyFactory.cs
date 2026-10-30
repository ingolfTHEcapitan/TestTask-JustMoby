using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Enemy.Components;
using _Project.Scripts.Logic.Enemy.Components.SatteMachine;
using _Project.Scripts.Logic.Enemy.Components.SatteMachine.States;
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

        public async UniTask<EnemyContainer> CreateEnemyAsync(Vector3 spawnPoint, Transform playerTransform)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.Enemy);
            
            EnemyContainer enemy = 
                _container.InstantiatePrefabForComponent<EnemyContainer>(prefab, spawnPoint, Quaternion.identity, _dynamicObjectsParent);

            EnemyRotateToPlayer rotateToPlayer = enemy.RotateToPlayer;
            rotateToPlayer.Initialize(playerTransform);

            EnemyStateMachine stateMachine = enemy.StateMachine;
            stateMachine.Initialize(spawnPoint, playerTransform, rotateToPlayer);
            
            Health health = enemy.Health;
            float maxHealth = _healthCalculator.CalculateEnemyMaxHealth();
            health.Initialize(maxHealth);
            
            _healthBarFactory.Create(enemy.HealthBarView, health);
            
            enemy.EnemyDeath.Initialize();
            
            EnemyAnimator enemyAnimator = enemy.Animator;
            enemyAnimator.Construct(stateMachine.GetState<EnemyAttackState>(), _pauseService);
            enemyAnimator.Initialize();
            return enemy;
        }
    }
}


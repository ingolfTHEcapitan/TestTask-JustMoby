using System;
using _Project.Scripts.Configs;
using _Project.Scripts.Enemy.States;
using _Project.Scripts.Infrastructure.Services.GamePause;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Common.States;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Enemy
{
    public class EnemyStateMachine: MonoBehaviour
    {
        private const float StoppingDistanceOffset = 0.5f;
        
        public event Action AttackEnded;
        public event Action Attack;
        public event Action SpawnAnimationEnded;

        [SerializeField] private TriggerObserver _triggerObserver;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;

        private IStateMachine _stateMachine;
        private IGamePauseService _pauseService;
        private EnemyConfig _config;
        private EnemyRotateToPlayer _enemyRotateToPlayer;
        private float _currentAttackCooldown;

        public bool AttackCooldownIsUp => _currentAttackCooldown <= 0f;
        
        [Inject]
        public void Construct(IGamePauseService pauseService, EnemyConfig config)
        {
            _pauseService = pauseService;
            _config = config;
        }
        
        public void Initialize(Vector3 spawnPoint, Transform playerTransform, EnemyRotateToPlayer enemyRotateToPlayer)
        {
            _agent.speed = _config.MoveSpeed;
            _agent.stoppingDistance = _config.AttackDistance - StoppingDistanceOffset;
            
            _stateMachine = new StateMachine();
            _stateMachine.RegisterState( new EnemyPatrolState(_stateMachine, enemy: this, _agent, _config, spawnPoint, _triggerObserver));
            _stateMachine.RegisterState( new EnemyAttackState(_stateMachine, enemy: this, _agent, _config, playerTransform, _triggerObserver, enemyRotateToPlayer, _animator));
            _stateMachine.RegisterState( new EnemyChaseState(_stateMachine, enemy: this, _agent, _config, playerTransform, _triggerObserver, enemyRotateToPlayer));
            
            _stateMachine.SetState<EnemyPatrolState>();
        }

        private void Update()
        { 
            if (_pauseService.IsPaused)
                return;
            
            _stateMachine.Update();
            UpdateAttackCoolDown();
        }

        private void OnDestroy() => 
            _stateMachine.InvokeDisposeOnStates();

        [UsedImplicitly]
        public void OnAttackEnded()
        {
            AttackEnded?.Invoke();
            _currentAttackCooldown = _config.AttackCooldown;
        }

        [UsedImplicitly]
        public void OnAttack() => 
            Attack?.Invoke();
        
        [UsedImplicitly]
        public void OnSpawnAnimationEnded() => 
            SpawnAnimationEnded?.Invoke();
        
        private void UpdateAttackCoolDown()
        {
            if (!AttackCooldownIsUp)
                _currentAttackCooldown -= Time.deltaTime;
        }
    }
}
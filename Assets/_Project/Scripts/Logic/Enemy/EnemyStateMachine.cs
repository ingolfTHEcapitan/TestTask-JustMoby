using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common.StateMachine;
using _Project.Scripts.Logic.Common.StateMachine.Transitions;
using _Project.Scripts.Logic.Enemy.States;
using _Project.Scripts.Services.GamePause;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Logic.Enemy
{
    public class EnemyStateMachine: MonoBehaviour
    {
        private const float StoppingDistanceOffset = 0.5f;
        
        [SerializeField] private NavMeshAgent _agent;
        
        private IGamePauseService _pauseService;
        private EnemyConfig _config;
        private EnemyRotateToPlayer _enemyRotateToPlayer;
        private Transform _playerTransform;
        private StateMachine _stateMachine;
        
        private float _currentAttackCooldown;
        private float _attackDistanceSquared;
        private float _chaseDistanceSquared;
        private bool _isSpawnAnimationEnded;
        
        [Inject]
        public void Construct(IGamePauseService pauseService, EnemyConfig config)
        {
            _pauseService = pauseService;
            _config = config;
        }
        
        public void Initialize(Vector3 spawnPoint, Transform playerTransform, EnemyRotateToPlayer enemyRotateToPlayer)
        {
            _playerTransform = playerTransform;
            _agent.speed = _config.MoveSpeed;
            _agent.stoppingDistance = _config.AttackDistance - StoppingDistanceOffset;
            
            _attackDistanceSquared = _config.AttackDistance * _config.AttackDistance;
            _chaseDistanceSquared = _config.ChaseDistance * _config.ChaseDistance;
            
            _stateMachine = new StateMachine();
            
            EnemyPatrolState patrolState = new EnemyPatrolState(_agent, _config, spawnPoint, new FuncPredicate(() => _isSpawnAnimationEnded));
            EnemyAttackState attackState = new EnemyAttackState(_agent, _config, enemyRotateToPlayer, new FuncPredicate(AttackCooldownIsUp), transform);
            EnemyChaseState chaseState = new EnemyChaseState(_agent, _config, enemyRotateToPlayer, playerTransform);
            
            _stateMachine.AddTransition(attackState, chaseState, new FuncPredicate(() => !IsPlayerInAttackRange()));
            _stateMachine.AddTransition(attackState, patrolState, new FuncPredicate(IsPlayerOutOfChaseRange));
            _stateMachine.AddTransition(chaseState, attackState, new FuncPredicate(IsPlayerInAttackRange));
            _stateMachine.AddTransition(chaseState, patrolState, new FuncPredicate(IsPlayerOutOfChaseRange));
            _stateMachine.AddTransition(patrolState, chaseState, new FuncPredicate(IsPlayerInChaseRange));
            
            _stateMachine.SetState(patrolState);
        }
        
        private void Update()
        { 
            if (_pauseService.IsPaused)
                return;
            
            _stateMachine.Update();
            UpdateAttackCoolDown();
        }
        
        [UsedImplicitly]
        public void OnAttackEnded()
        {
            if (_stateMachine.CurrentState is EnemyAttackState attackState) 
                attackState.AttackEnded();
            
            _currentAttackCooldown = _config.AttackCooldown;
        }

        [UsedImplicitly]
        public void OnAttack()
        {
            if (_stateMachine.CurrentState is EnemyAttackState attackState) 
                attackState.DealDamage();
        }

        [UsedImplicitly]
        public void OnSpawnAnimationEnded() => 
            _isSpawnAnimationEnded = true;
        
        public TState GetState<TState>() where TState: EnemyBaseState => 
            _stateMachine.GetState<TState>() as TState;
        
        private void UpdateAttackCoolDown()
        {
            if (!AttackCooldownIsUp())
                _currentAttackCooldown -= Time.deltaTime;
        }
        
        private bool IsPlayerInAttackRange() => 
            GetSqrDistanceToPlayer() <= _attackDistanceSquared;

        private bool IsPlayerInChaseRange()
        {
            float sqrDistance = GetSqrDistanceToPlayer();
            return sqrDistance > _attackDistanceSquared && sqrDistance <= _chaseDistanceSquared;
        }

        private bool IsPlayerOutOfChaseRange() => 
            GetSqrDistanceToPlayer() > _chaseDistanceSquared;

        private bool AttackCooldownIsUp() 
            => _currentAttackCooldown <= 0f;

        private float GetSqrDistanceToPlayer() => 
            (transform.position - _playerTransform.position).sqrMagnitude;
    }
}
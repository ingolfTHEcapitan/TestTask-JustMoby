using System;
using System.Linq;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Common.States;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Enemy.States
{
    public class EnemyAttackState : State, IDisposable
    {
        private const string PlayerLayer = "Player";
        private const float AttackYOffset = 0.5f;
        private const float DebugLifeTime = 3;

        private readonly IStateMachine _stateMachine;
        private readonly EnemyStateMachine _enemy;
        private readonly EnemyConfig _config;
        private readonly Animator _animator;
        private readonly NavMeshAgent _agent;
        private readonly TriggerObserver _triggerObserver;
        private readonly Transform _playerTransform;
        private readonly EnemyRotateToPlayer _enemyRotateToPlayer;

        private readonly Collider[] _hits = new Collider[1];
        private readonly int _attackHash = Animator.StringToHash("Attack");

        private bool _isAttacking;
        private int _layerMask;

        public EnemyAttackState(IStateMachine stateMachine, EnemyStateMachine enemy, NavMeshAgent agent,
            EnemyConfig config, Transform playerTransform, TriggerObserver triggerObserver,
            EnemyRotateToPlayer enemyRotateToPlayer, Animator animator) : base(stateMachine)
        {
            _stateMachine = stateMachine;
            _enemy = enemy;
            _agent = agent;
            _config = config;
            _playerTransform = playerTransform;
            _triggerObserver = triggerObserver;
            _enemyRotateToPlayer = enemyRotateToPlayer;
            _animator = animator;

            Initialize();
        }

        private void Initialize()
        {
            _enemy.Attack += OnAttack;
            _enemy.AttackEnded += OnAttackEnded;
            _triggerObserver.TriggerExit += OnTriggerExit;
            _layerMask = LayerMask.GetMask(PlayerLayer);
        }

        public void Dispose()
        {
            _enemy.Attack -= OnAttack;
            _enemy.AttackEnded -= OnAttackEnded;
            _triggerObserver.TriggerExit -= OnTriggerExit;
        }

        public override void Enter()
        {
            _agent.ResetPath();
            _enemyRotateToPlayer.enabled = true;
        }

        public override void Update()
        {
            if (CanAttack())
                StartAttack();

            if (!IsPlayerInAttackRange())
                _stateMachine.SetState<EnemyChaseState>();
        }

        private void OnTriggerExit(Collider obj)
        {
            _stateMachine.SetState<EnemyPatrolState>();
            _enemyRotateToPlayer.enabled = false;
        }

        private void StartAttack()
        {
            _enemy.transform.LookAt(_playerTransform);
            _animator.SetTrigger(_attackHash);
            _isAttacking = true;
        }

        private void OnAttack()
        {
            PhysicsDebug.DrawDebugSphere(GetStartPoint(), _config.AttackRadius, DebugLifeTime, Color.red);
            if (Hit(out Collider hit))
            {
                PhysicsDebug.DrawDebugSphere(GetStartPoint(), _config.AttackRadius, DebugLifeTime, Color.green);

                IHealth playerHealth = hit.GetComponent<IHealth>();
                playerHealth.TakeDamage(_config.AttackDamage);
            }
        }
        
        private void OnAttackEnded() =>
            _isAttacking = false;

        private bool Hit(out Collider hit)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(GetStartPoint(), _config.AttackRadius, _hits, _layerMask);
            hit = _hits.FirstOrDefault();
            return hitCount > 0;
        }

        private Vector3 GetStartPoint()
        {
            return new Vector3(_enemy.transform.position.x, _enemy.transform.position.y + AttackYOffset,
                       _enemy.transform.position.z) +
                   _enemy.transform.forward * _config.AttackDistance;
        }

        private bool IsPlayerInAttackRange()
        {
            float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _playerTransform.position);
            return distanceToPlayer <= _config.AttackDistance;
        }

        private bool CanAttack() =>
            !_isAttacking && _enemy.AttackCooldownIsUp;
    }
}
using System;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Common.States;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Logic.Enemy.States
{
    public class EnemyChaseState : State, IDisposable
    {
        private readonly IStateMachine _stateMachine;
        private readonly EnemyStateMachine _enemy;
        private readonly NavMeshAgent _agent;
        private readonly EnemyConfig _config;
        private readonly Transform _playerTransform;
        private readonly TriggerObserver _triggerObserver;
        private readonly EnemyRotateToPlayer _enemyRotateToPlayer;

        public EnemyChaseState(IStateMachine stateMachine, EnemyStateMachine enemy, NavMeshAgent agent,
            EnemyConfig config, Transform playerTransform, TriggerObserver triggerObserver,
            EnemyRotateToPlayer enemyRotateToPlayer) : base(stateMachine)
        {
            _stateMachine = stateMachine;
            _enemy = enemy;
            _agent = agent;
            _config = config;
            _playerTransform = playerTransform;
            _triggerObserver = triggerObserver;
            _enemyRotateToPlayer = enemyRotateToPlayer;

            Initialize();
        }

        private void Initialize() => 
            _triggerObserver.TriggerExit += OnTriggerExit;

        public void Dispose() => 
            _triggerObserver.TriggerExit -= OnTriggerExit;

        public override void Enter()
        {
            _agent.ResetPath();
            _enemyRotateToPlayer.enabled = true;
        }

        public override void Update()
        {
            _agent.SetDestination(_playerTransform.position);

            if (IsPlayerInAttackRange()) 
                _stateMachine.SetState<EnemyAttackState>();
        }

        private void OnTriggerExit(Collider obj)
        {
            _stateMachine.SetState<EnemyPatrolState>();
            _enemyRotateToPlayer.enabled = false;
        }

        private bool IsPlayerInAttackRange()
        {
            float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _playerTransform.position);
            return distanceToPlayer <= _config.AttackDistance;
        }
    }
}
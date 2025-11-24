using System;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Common.States;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Logic.Enemy.States
{
    public class EnemyPatrolState: State, IDisposable
    {
        private const float MaxSampleDistance = 4f;

        private readonly IStateMachine _stateMachine;
        private readonly EnemyStateMachine _enemy;
        private readonly NavMeshAgent _agent;
        private readonly EnemyConfig _config;
        private readonly Vector3 _spawnPoint;
        private readonly TriggerObserver _triggerObserver;

        private bool IsSpawnAnimationEnded;
        private bool _movementIsActive;

        public EnemyPatrolState(IStateMachine stateMachine, EnemyStateMachine enemy, NavMeshAgent agent,
            EnemyConfig config, Vector3 spawnPoint, TriggerObserver triggerObserver) : base(stateMachine)
        {
            _stateMachine = stateMachine;
            _enemy = enemy;
            _agent = agent;
            _config = config;
            _spawnPoint = spawnPoint;
            _triggerObserver = triggerObserver;

            Initialize();
        }

        private void Initialize()
        {
            _enemy.SpawnAnimationEnded += OnSpawnAnimationEnded;
            _triggerObserver.TriggerEnter += OnTriggerEnter;
        }

        public void Dispose()
        {
            _enemy.SpawnAnimationEnded -= OnSpawnAnimationEnded;
            _triggerObserver.TriggerEnter -= OnTriggerEnter;
        }

        public override void Enter()
        {
            if (!IsSpawnAnimationEnded)
                return;
            
            PatrolRandomPoint();
        }

        public override void Update()
        {
            if (!IsSpawnAnimationEnded)
                return;
            
            if (_agent.remainingDistance <= _agent.stoppingDistance) 
                PatrolRandomPoint();
        }

        private void OnTriggerEnter(Collider other) => 
            _stateMachine.SetState<EnemyChaseState>();

        private void PatrolRandomPoint()
        {
            Vector3 targetPosition = GetRandomPosition(_spawnPoint);
            _agent.SetDestination(targetPosition);
        }

        private Vector3 GetRandomPosition(Vector3 moveAreaCenter)
        {
            Vector3 randomPoint = GetRandomPoint(moveAreaCenter);
            
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit closestPoint, MaxSampleDistance, NavMesh.AllAreas))
                return closestPoint.position;
            
            return moveAreaCenter;
        }

        private Vector3 GetRandomPoint(Vector3 centerPoint)
        {
            Vector2 randomDirection = Random.insideUnitCircle * Random.Range(_config.MinPatrolDistance, _config.MaxPatrolDistance);
            Vector3 offset = new Vector3(randomDirection.x, 0, randomDirection.y);
            return centerPoint + offset;
        }

        private void OnSpawnAnimationEnded() => 
            IsSpawnAnimationEnded = true;
    }
}
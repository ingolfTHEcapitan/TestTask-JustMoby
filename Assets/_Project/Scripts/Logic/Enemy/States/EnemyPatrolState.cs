using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common.StateMachine.Transitions;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Logic.Enemy.States
{
    public class EnemyPatrolState: EnemyBaseState
    {
        private const float MaxSampleDistance = 4f;

        private readonly Vector3 _spawnPoint;
        private readonly EnemyStateMachine _enemy;
        private readonly IPredicate _isSpawnAnimationEnded;

        public EnemyPatrolState(NavMeshAgent agent, EnemyConfig config, Vector3 spawnPoint, 
            IPredicate isSpawnAnimationEnded) : base(agent, config)
        {
            _isSpawnAnimationEnded = isSpawnAnimationEnded;
            _spawnPoint = spawnPoint;
        }

        public override void OnEnter()
        {
            if (!_isSpawnAnimationEnded.Evaluate())
                return;
            
            PatrolRandomPoint();
        }

        public override void Update()
        {
            if (!_isSpawnAnimationEnded.Evaluate())
                return;
            
            if (_agent.remainingDistance <= _agent.stoppingDistance) 
                PatrolRandomPoint();
        }

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
    }
}
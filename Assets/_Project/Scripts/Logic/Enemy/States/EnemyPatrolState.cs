using _Project.Scripts.Configs;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Logic.Enemy.States
{
    public class EnemyPatrolState: EnemyBaseState
    {
        private const float MaxSampleDistance = 4f;

        private readonly Vector3 _spawnPoint;

        public EnemyPatrolState(NavMeshAgent agent, EnemyConfig config, Vector3 spawnPoint) : base(agent, config) => 
            _spawnPoint = spawnPoint;

        public override void OnEnter()
        {
            base.OnEnter();
            PatrolRandomPoint();
        }

        public override void Update()
        {
            base.Update();
            
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
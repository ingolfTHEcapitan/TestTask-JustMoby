using _Project._Scripts.Infrastructure.Services.GamePause;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace _Project._Scripts.Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        private const float MaxSampleDistance = 4f;
        
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _minMoveDistance = 7f;
        [SerializeField] private float _maxMoveDistance = 15f;
        
        private Vector3 _spawnPoint;
        private IGamePauseService _pauseService;
        private bool IsSpawnAnimationEnded;

        public void Construct(IGamePauseService pauseService) => 
            _pauseService = pauseService;

        public void Initialize(Vector3 spawnPoint) => 
            _spawnPoint = spawnPoint;

        private void Update()
        {
            if(_pauseService.IsPaused || !IsSpawnAnimationEnded)
                return;
            
            Move();
        }

        [UsedImplicitly]
        public void OnSpawnAnimationEnded() => 
            IsSpawnAnimationEnded = true;

        private void Move()
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                Vector3 targetPosition = GetRandomPosition(_spawnPoint);
                _agent.SetDestination(targetPosition);
            }
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
            Vector2 randomDirection = Random.insideUnitCircle * Random.Range(_minMoveDistance, _maxMoveDistance);
            Vector3 offset = new Vector3(randomDirection.x, 0, randomDirection.y);
            return centerPoint + offset;
        }
    }
}
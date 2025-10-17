using _Project._Scripts.Services.GamePause;
using UnityEngine;
using UnityEngine.AI;

namespace _Project._Scripts.Enemy
{
    public class EnemyAgent : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        
        [SerializeField] private float _minRoamingDistance;
        [SerializeField] private float _maxRoamingDistance;
        [SerializeField] private float _maxRoamingTime = 3f;
        
        private float _roamingTime;
        private Vector3 _currentPosition;
        private Vector3 _roamingPosition;
        private IGamePauseService _pauseService;

        public void Construct(IGamePauseService pauseService) => 
            _pauseService = pauseService;
        
        private void Update()
        {
            if(_pauseService.IsPaused)
                return;
            
            Roaming();
        }
        
        private void Roaming()
        {
            _roamingTime -= Time.deltaTime;

            if (_roamingTime <= 0)
            {
                _currentPosition = transform.position;
                _roamingPosition = GetRoamingPosition(_currentPosition);
                _agent.SetDestination(_roamingPosition);
                _roamingTime = _maxRoamingTime;
            }
        }

        private Vector3 GetRoamingPosition(Vector3 currentPosition) => 
            currentPosition + GetRandomDirection() * Random.Range(_minRoamingDistance, _maxRoamingDistance);

        private Vector3 GetRandomDirection() => 
            new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
    }
}
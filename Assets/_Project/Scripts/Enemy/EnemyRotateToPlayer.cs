using _Project.Scripts.Configs;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Enemy
{
    public class EnemyRotateToPlayer: MonoBehaviour
    {
        private Transform _playerTransform;
        private Vector3 _positionToLook;
        private EnemyConfig _config;

        [Inject]
        public void Construct(EnemyConfig config) => 
            _config = config;

        public void Initialize(Transform playerTransform) => 
            _playerTransform = playerTransform;

        private void Update() =>
            RotateTowardsPlayer();

        private void RotateTowardsPlayer() => 
            transform.rotation = SmoothRotation(transform.rotation, GetPositionToLook());

        private Quaternion SmoothRotation(Quaternion currentRotation, Vector3 positionToLook) => 
            Quaternion.Lerp(currentRotation, TargetRotation(positionToLook), _config.RotationSpeed * Time.deltaTime);

        private static Quaternion TargetRotation(Vector3 positionToLook) => 
            Quaternion.LookRotation(positionToLook);

        private Vector3 GetPositionToLook()
        {
            Vector3 directionToPlayer = _playerTransform.position - transform.position;
            return new Vector3(directionToPlayer.x, transform.position.y, directionToPlayer.z);
        }
    }
}
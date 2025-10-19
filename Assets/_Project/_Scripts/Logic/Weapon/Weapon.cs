using _Project._Scripts.Infrastructure.Services.Factory;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.PlayerInput;
using UnityEngine;

namespace _Project._Scripts.Logic.Weapon
{
    public class Weapon : MonoBehaviour
    {
        private const float MaxRayDistance = 100f;
        
        [SerializeField] private Transform _shootPoint;
        [SerializeField, Space] private float _fireRate;
        
        private float _nextTimeToFire;
        private Camera _camera;
        private Transform _bulletParent;
        private IGamePauseService _pauseService;
        private IInputService _inputService;
        private IGameFactory _factory;

        public void Construct(IGamePauseService pauseService,
            IInputService inputService, IGameFactory factory)
        {
            _pauseService = pauseService;
            _inputService = inputService;
            _factory = factory;
        }

        private void Start() => 
            _camera = Camera.main;

        private void Update()
        {
            if (_pauseService.IsPaused)
                return;
            
            if (_inputService.IsFireButtonPressed() && CanShoot()) 
                Shoot();
        }

        private void Shoot()
        {
            _nextTimeToFire = Time.time + 1 / _fireRate;
            _factory.CreateBullet(_shootPoint.position, GetShootDirection());
        }

        private Vector3 GetShootDirection()
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 shootDirection = (GetTargetPoint(ray) - _shootPoint.position).normalized;
            return shootDirection;
        }
        
        private Vector3 GetTargetPoint(Ray ray)
        {
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out RaycastHit hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(MaxRayDistance);
            return targetPoint;
        }

        private bool CanShoot() => 
            Time.time > _nextTimeToFire;
    }
}
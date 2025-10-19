using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.PlayerInput;
using _Project._Scripts.Logic.PlayerStats;
using UnityEngine;

namespace _Project._Scripts.Logic.Weapon
{
    public class Weapon : MonoBehaviour
    {
        private const float MaxRayDistance = 100f;
        
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _shootPoint;
        [SerializeField, Space] private float _fireRate;
        
        private float _nextTimeToFire;
        private Camera _camera;
        private Transform _bulletParent;
        private PlayerStatsModel _playerStatsModel;
        private IGamePauseService _pauseService;
        private IInputService _inputService;

        public void Construct(PlayerStatsModel playerStatsModel, IGamePauseService pauseService,
            IInputService inputService, Transform bulletParent)
        {
            _playerStatsModel = playerStatsModel;
            _pauseService = pauseService;
            _inputService = inputService;
            _bulletParent = bulletParent;
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
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 shootDirection = (GetTargetPoint(ray) - _shootPoint.position).normalized;
            
            _nextTimeToFire = Time.time + 1 / _fireRate;
            
            Bullet bullet = Instantiate(_bulletPrefab, _shootPoint.position, Quaternion.identity);
            float damage = _playerStatsModel.GetStatValue(StatName.Damage);
            bullet.Initialize(shootDirection, damage, _bulletParent);
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
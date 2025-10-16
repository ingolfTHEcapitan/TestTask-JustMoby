using _Project._Scripts.Player.StatSystem;
using UnityEngine;

namespace _Project._Scripts.Weapon
{
    public class Weapon : MonoBehaviour
    {
        private const float MaxRayDistance = 100f;
        
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private Transform _bulletParent;
        [SerializeField, Header("Settings")] private float _fireRate;
        
        private float _nextTimeToFire;
        private Camera _camera;
        private PlayerStatsSystem _playerStatsSystem;

        public void Construct(PlayerStatsSystem playerStatsSystem)
        {
            _playerStatsSystem = playerStatsSystem;
        }

        private void Start() => 
            _camera = Camera.main;

        private void Update()
        {
            if (Input.GetButton("Fire1") && CanShoot()) 
                Shoot();
        }

        private void Shoot()
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 shootDirection = (GetTargetPoint(ray) - _shootPoint.position).normalized;
            
            _nextTimeToFire = Time.time + 1 / _fireRate;
            
            Bullet bullet = Instantiate(_bulletPrefab, _shootPoint.position, Quaternion.identity);
            float damage = _playerStatsSystem.GetStatValue(StatName.Damage);
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
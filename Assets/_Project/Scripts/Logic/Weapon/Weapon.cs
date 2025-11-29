using System;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Services.Factory.BulletFactory;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.Statistics;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Weapon
{
    public class Weapon : MonoBehaviour
    {
        private const float MaxRayDistance = 100f;
        private const int AllLayers = -1;

        [SerializeField] private Transform _shootPoint;
        
        private float _fireRate;
        private float _nextTimeToFire;
        private Camera _playerCamera;
        private IGamePauseService _pauseService;
        private IInputService _inputService;
        private IBulletFactory _factory;
        private IGameStatistics _statistics;
        private WeaponConfig _config;
        
        [Inject]
        public void Construct(IGamePauseService pauseService, IInputService inputService, 
            IBulletFactory factory, IGameStatistics statistics, WeaponConfig config)
        {
            _pauseService = pauseService;
            _inputService = inputService;
            _factory = factory;
            _statistics = statistics;
            _config = config;
        }
        
        public void Initialize(Camera playerCamera)
        {
            _playerCamera = playerCamera;
            _fireRate = _config.FireRate;
        }

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
            _factory.CreateBullet(_config.BulletConfig, _shootPoint, GetShootDirection());
            _statistics.RecordShot();
        }

        private Vector3 GetShootDirection()
        {
            Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 shootDirection = (GetTargetPoint(ray) - _shootPoint.position).normalized;
            return shootDirection;
        }
        
        private Vector3 GetTargetPoint(Ray ray)
        {
            Vector3 targetPoint;
            
            // используем QueryTriggerInteraction, что бы луч при выстреле игнорировал триггеры.
            if (Physics.Raycast(ray, out RaycastHit hit, MaxRayDistance ,AllLayers, QueryTriggerInteraction.Ignore))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(MaxRayDistance);
            return targetPoint;
        }

        private bool CanShoot() => 
            Time.time > _nextTimeToFire;
    }
}
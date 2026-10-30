using System;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.Player.Weapon.Bullet;
using _Project.Scripts.Services;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.PlayerInput;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.Weapon
{
    public class Weapon : MonoBehaviour
    {
        public event Action<Vector3, Transform> OnBulletHit;
        public event Action OnShoot;
        
        private const float MaxRayDistance = 100f;
        private const int AllLayers = -1;

        [SerializeField] private Transform _shootPoint;
        
        private float _fireRate;
        private float _nextTimeToFire;
        private readonly Vector3 _screenCenter = new Vector3(0.5f, 0.5f, 0f);
        
        private Camera _playerCamera;
        private IGamePauseService _pauseService;
        private IInputService _inputService;
        private BulletFactory _factory;
        private GameStatistics _statistics;
        private WeaponConfig _weaponConfig;
        private BulletConfig _bulletConfig;

        [Inject]
        private void Construct(IGamePauseService pauseService, IInputService inputService, BulletFactory factory, 
            GameStatistics statistics, WeaponConfig weaponConfig, BulletConfig bulletConfig)
        {
            _pauseService = pauseService;
            _inputService = inputService;
            _factory = factory;
            _statistics = statistics;
            _weaponConfig = weaponConfig;
            _bulletConfig = bulletConfig;
        }
        
        public void Initialize(Camera playerCamera)
        {
            _pauseService.OnPausableUpdate += Shoot;
            _playerCamera = playerCamera;
            _fireRate = _weaponConfig.FireRate;
        }

        private void OnDestroy() => 
            _pauseService.OnPausableUpdate -= Shoot;

        private void Shoot()
        {
            if (_inputService.IsFireButtonPressed() && CanShoot())
            {
                ShootAsync().Forget(Debug.LogException);
                OnShoot?.Invoke();
                _statistics.RecordShot();
            }
        }

        private async UniTask ShootAsync()
        {
            _nextTimeToFire = Time.time + 1 / _fireRate;
            Ray ray = _playerCamera.ViewportPointToRay(_screenCenter);
            Vector3 targetPoint = GetTargetPoint(ray);
            
            Bullet.Bullet bullet = await _factory.CreateBulletAsync(_bulletConfig, _shootPoint, GetShootDirection(targetPoint), targetPoint);
            bullet.OnBulletDestroy += UnSubscribe;
            bullet.OnHit += InvokeOnBulletHit;
            return;

            void UnSubscribe()
            {
                bullet.OnHit -= InvokeOnBulletHit;
                bullet.OnBulletDestroy -= UnSubscribe;
            }
        }

        private void InvokeOnBulletHit(Vector3 hitPosition, Transform targetContainer) => 
            OnBulletHit?.Invoke(hitPosition, targetContainer);
        
        private Vector3 GetShootDirection(Vector3 targetPoint)
        {
            return (targetPoint - _shootPoint.position).normalized;
        }
        
        private Vector3 GetTargetPoint(Ray ray)
        {
            Vector3 targetPoint;
            
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
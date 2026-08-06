using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.Player.Weapon.Bullet.Factory;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.Sound;
using _Project.Scripts.Services.Statistics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.Weapon
{
    public class Weapon : MonoBehaviour
    {
        private const float MaxRayDistance = 100f;
        private const int AllLayers = -1;

        [SerializeField] private Transform _shootPoint;
        [SerializeField] private GameObject _hitFxPrefab;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _shotSound;
        
        private float _fireRate;
        private float _nextTimeToFire;
        private readonly Vector3 _screenCenter = new Vector3(0.5f, 0.5f, 0f);
        
        private Camera _playerCamera;
        private IGamePauseService _pauseService;
        private IInputService _inputService;
        private IBulletFactory _factory;
        private IGameStatistics _statistics;
        private IAudioService _audioService;
        private WeaponConfig _weaponConfig;
        private BulletConfig _bulletConfig;

        [Inject]
        private void Construct(IGamePauseService pauseService, IInputService inputService, IBulletFactory factory, 
            IGameStatistics statistics, IAudioService audioService, WeaponConfig weaponConfig, BulletConfig bulletConfig)
        {
            _pauseService = pauseService;
            _inputService = inputService;
            _factory = factory;
            _statistics = statistics;
            _audioService = audioService;
            _weaponConfig = weaponConfig;
            _bulletConfig = bulletConfig;
        }
        
        public void Initialize(Camera playerCamera)
        {
            _playerCamera = playerCamera;
            _fireRate = _weaponConfig.FireRate;
        }

        private async void Update()
        {
            if (_pauseService.IsPaused)
                return;

            if (_inputService.IsFireButtonPressed() && CanShoot())
            {
                await ShootAsync();
                _statistics.RecordShot();
                _audioService.PlayOneShot(_shotSound, _audioSource);
            }
                
        }

        private async UniTask ShootAsync()
        {
            _nextTimeToFire = Time.time + 1 / _fireRate;
            Ray ray = _playerCamera.ViewportPointToRay(_screenCenter);
            Vector3 targetPoint = GetTargetPoint(ray);
            await _factory.CreateBulletAsync(_bulletConfig, _shootPoint, GetShootDirection(targetPoint), targetPoint);
        }

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
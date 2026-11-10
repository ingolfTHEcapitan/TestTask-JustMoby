using System;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.GamePause;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.Weapon.Bullet
{
    public class Bullet : MonoBehaviour
    {
        public event Action<Vector3, Transform> OnHit;
        public event Action OnBulletDisable;
        private Action<Bullet> _onReturnToPool;

        private Vector3 _direction;
        private float _damage;
        private float _speed;
        private float _lifeTime;
        private float _currentLifeTime;
        private Vector3 _targetPoint;

        private IGamePauseService _pauseService;

        [Inject]
        private void Construct(IGamePauseService pauseService) => 
            _pauseService = pauseService;

        public void Initialize(BulletConfig config, Transform at, Vector3 direction, Vector3 targetPoint, float damage, 
            Transform parent, Action<Bullet> OnReturnToPool)
        {
            transform.SetPositionAndRotation(at.position, at.rotation);
            _direction = direction;
            _targetPoint = targetPoint;
            _damage = damage;
            _speed = config.Speed;
            _lifeTime = config.LifeTime;
            _currentLifeTime = 0;
            transform.SetParent(parent);
            _onReturnToPool = OnReturnToPool;
        }

        private void OnEnable()
        {
            _pauseService.OnPausableUpdate += PausableUpdate;
        }

        private void OnDisable()
        {
            OnBulletDisable?.Invoke();
            _pauseService.OnPausableUpdate -= PausableUpdate;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out IDamageable enemy))
            {
                enemy.TakeDamage(_damage);
                OnHit?.Invoke(_targetPoint, other.gameObject.transform);
                _onReturnToPool?.Invoke(this);
            }
        }

        private void PausableUpdate()
        {
            MoveBullet();
            UpdateLifeTime();
        }

        private void UpdateLifeTime()
        {
            _currentLifeTime += Time.deltaTime;

            if (_currentLifeTime >= _lifeTime)
                _onReturnToPool?.Invoke(this);
        }

        private void MoveBullet() => 
            transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
    }
}
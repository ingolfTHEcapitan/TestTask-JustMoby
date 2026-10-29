using System;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services;
using _Project.Scripts.Services.GamePause;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.Weapon.Bullet
{
    public class Bullet : MonoBehaviour
    {
        private Vector3 _direction;
        private float _damage;
        private float _speed;
        private float _lifeTime;
        private float _currentLifeTime;
        private Vector3 _targetPoint;

        private EffectsService _effectsService;
        private IGamePauseService _pauseService;

        [Inject]
        private void Construct(EffectsService effectsService, IGamePauseService pauseService)
        {
            _effectsService = effectsService;
            _pauseService = pauseService;
        }

        public void Initialize(BulletConfig config, Vector3 direction, Vector3 targetPoint, float damage, Transform parent)
        {
            _pauseService.OnPausableUpdate += PausableUpdate;
            
            _direction = direction;
            _targetPoint = targetPoint;
            _damage = damage;
            _speed = config.Speed;
            _lifeTime = config.LifeTime;
            _currentLifeTime = 0;
            transform.SetParent(parent);
        }

        private void OnDestroy() => 
            _pauseService.OnPausableUpdate -= PausableUpdate;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
                _effectsService.PlayHitFxAsync(_targetPoint, other.gameObject.transform).Forget(Debug.LogException);
                Destroy(gameObject);
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
                Destroy(gameObject);
        }

        private void MoveBullet() => 
            transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
    }
}
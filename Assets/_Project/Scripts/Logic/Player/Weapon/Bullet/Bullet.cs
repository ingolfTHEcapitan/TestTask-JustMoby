using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.Effects;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.Weapon.Bullet
{
    public class Bullet: MonoBehaviour
    {
        private Vector3 _direction;
        private float _damage;
        private float _speed;
        private float _lifeTime;
        private Vector3 _targetPoint;
        
        private IEffectsService _effectsService;

        [Inject]
        public void Construct(IEffectsService effectsService) => 
            _effectsService = effectsService;

        public void Initialize(BulletConfig config, Vector3 direction, Vector3 targetPoint, float damage, Transform parent)
        {
            _direction = direction;
            _targetPoint = targetPoint;
            _damage = damage;
            _speed = config.Speed;
            _lifeTime = config.LifeTime;
            transform.SetParent(parent);
            
            DestroyBullet(_lifeTime);
        }
        
        private void Update() => 
            transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);

        private async void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out IHealth health))
            {
                health.TakeDamage(_damage);
                await _effectsService.PlayHitFx(_targetPoint, other.gameObject.transform);
                DestroyBullet();
            }
        }

        private void DestroyBullet(float lifeTime = 0f) => 
            Destroy(gameObject, lifeTime);
    }
}
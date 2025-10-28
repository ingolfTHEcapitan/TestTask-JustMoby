using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.Common;
using UnityEngine;

namespace _Project.Scripts.Logic.Weapon
{
    public class Bullet: MonoBehaviour
    {
        private Vector3 _direction;
        private float _damage;
        private float _speed;
        private float _lifeTime;

        public void Initialize(BulletConfig config, Vector3 direction, float damage, Transform parent)
        {
            _direction = direction;
            _damage = damage;
            _speed = config.Speed;
            _lifeTime = config.LifeTime;
            transform.SetParent(parent);
            
            DestroyBullet(_lifeTime);
        }
        
        private void Update() => 
            transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out IHealth health))
            {
                health.TakeDamage(_damage);
                DestroyBullet();
            }
        }

        private void DestroyBullet(float lifeTime = 0f) => 
            Destroy(gameObject, lifeTime);
    }
}
using UnityEngine;

namespace _Project._Scripts.Logic.Weapon
{
    internal class Bullet: MonoBehaviour
    {
        [SerializeField] private float _speed = 8f;
        [SerializeField] private float _lifeTime = 5f;
        
        private Vector3 _direction;
        private float _damage;
        
        public void Initialize(Vector3 direction, float damage, Transform parent)
        {
            _direction = direction;
            _damage = damage;
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
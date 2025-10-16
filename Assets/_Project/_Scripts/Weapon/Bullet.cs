using System;
using _Project._Scripts.Enemy;
using UnityEngine;

namespace _Project._Scripts.Weapon
{
    internal class Bullet: MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _speed = 8f;
        [SerializeField] private float _lifeTime = 5f;
        
        private Vector3 _direction;
        private float _damage;

        private void Awake() => 
            DestroyBullet(_lifeTime);

        public void Initialize(Vector3 direction, float damage, Transform parent)
        {
            _direction = direction;
            _damage = damage;
            transform.SetParent(parent);
        }
        
        private void FixedUpdate()
        {
            transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(_damage);
                DestroyBullet();
            }
        }

        private void DestroyBullet(float lifeTime = 0f) => 
            Destroy(gameObject, lifeTime);
    }
}
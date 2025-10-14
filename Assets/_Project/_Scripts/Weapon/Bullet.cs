using System;
using _Project._Scripts.Enemy;
using UnityEngine;

namespace _Project._Scripts.Weapon
{
    internal class Bullet: MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _speed = 8f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _lifeTime = 5f;
        
        private Vector3 _direction;

        private void Awake() => 
            DestroyBullet(_lifeTime);

        public void SetDirection(Vector3 direction)
        {
            _direction = direction;
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
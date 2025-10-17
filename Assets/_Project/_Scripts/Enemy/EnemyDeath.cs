using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _Project._Scripts.Enemy
{
    public class EnemyDeath: MonoBehaviour
    {
        private static readonly int Death = Animator.StringToHash("Die");
        
        [SerializeField] private EnemyHealth _health;
        [SerializeField] private Animator _animator;
        [SerializeField] private NavMeshAgent _agent;

        private const float _destroyDelay = 2f;

        public event Action<EnemyDeath> OnDied;

        private void Start() => 
            _health.OnHealthChanged += OnOnHealthChanged;

        private void OnDestroy()
        {
            _health.OnHealthChanged -= OnOnHealthChanged;
        }

        private void OnOnHealthChanged()
        {
            if (_health.Current <= 0)
                Die();
        }

        private void Die()
        {
            _health.OnHealthChanged -= OnOnHealthChanged;
            _agent.speed = 0f;
            _animator.SetTrigger(Death);

            StartCoroutine(DestroyTimer(_destroyDelay));
            OnDied?.Invoke(this);
        }

        private IEnumerator DestroyTimer(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}
using System;
using System.Collections;
using _Project._Scripts.Logic.PlayerStats;
using UnityEngine;
using UnityEngine.AI;

namespace _Project._Scripts.Enemy
{
    public class EnemyDeath: MonoBehaviour
    {
        private readonly int DieHash = Animator.StringToHash("Die");
        
        public event Action<EnemyDeath> OnDied;
        
        [SerializeField] private EnemyHealth _health;
        [SerializeField] private Animator _animator;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _destroyDelay = 1.5f;
        
        private PlayerStatsModel _playerStatsModel;
        
        public void Construct(PlayerStatsModel playerStatsModel) => 
            _playerStatsModel = playerStatsModel;

        private void Start() => 
            _health.OnHealthChanged += OnOnHealthChanged;

        private void OnDestroy() => 
            _health.OnHealthChanged -= OnOnHealthChanged;

        private void OnOnHealthChanged()
        {
            if (_health.CurrentHealth <= 0)
                Die();
        }

        private void Die()
        {
            _health.OnHealthChanged -= OnOnHealthChanged;
            _agent.speed = 0f;
            _animator.SetTrigger(DieHash);
            _playerStatsModel.AddUpgradePoint();
            
            StartCoroutine(DestroyTimer(_destroyDelay));
        }
        
        private IEnumerator DestroyTimer(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
            OnDied?.Invoke(this);
        }
    }
}
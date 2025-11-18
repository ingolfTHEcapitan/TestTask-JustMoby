using System;
using System.Collections;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.PlayerStats;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Enemy
{
    public class EnemyDeath: MonoBehaviour
    {
        public event Action<EnemyDeath> OnDied;
        
        [SerializeField] private Health _health;
        [SerializeField] private Animator _animator;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _destroyDelay = 1.5f;

        private readonly int _dieHash = Animator.StringToHash("Die");
        private readonly int _hitHash = Animator.StringToHash("Hit");
        private PlayerStatsModel _playerStatsModel;
        
        [Inject]
        public void Construct(PlayerStatsModel playerStatsModel) => 
            _playerStatsModel = playerStatsModel;

        public void Initialize() => 
            _health.OnHealthChanged += OnOnHealthChanged;

        private void OnDestroy() => 
            _health.OnHealthChanged -= OnOnHealthChanged;

        private void OnOnHealthChanged()
        {
            if (_health.CurrentHealth <= 0)
                Die();
            
            _animator.SetTrigger(_hitHash);
        }

        private void Die()
        {
            _health.OnHealthChanged -= OnOnHealthChanged;
            _agent.speed = 0f;
            _animator.SetTrigger(_dieHash);
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
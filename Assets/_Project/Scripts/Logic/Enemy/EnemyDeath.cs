using System;
using System.Collections;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.PlayerStats;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Logic.Enemy
{
    public class EnemyDeath: MonoBehaviour
    {
        public event Action<EnemyDeath> OnDied;

        [SerializeField] private Health _health;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _destroyDelay = 1.5f;
        [Header("Components To Disable On Death")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EnemyStateMachine _enemyStateMachine;
        [SerializeField] private EnemyRotateToPlayer _enemyRotateToPlayer;
        [SerializeField] private SphereCollider _attackZoneTrigger;

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
        
        public void KillEnemy() => 
            _health.TakeDamage(_health.MaxHealth);

        private void OnOnHealthChanged()
        {
            if (_health.CurrentHealth <= 0)
                Die();
            
            _animator.SetTrigger(_hitHash);
        }

        private void Die()
        {
            _health.OnHealthChanged -= OnOnHealthChanged;
            DisableEnemyComponents();
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

        private void DisableEnemyComponents()
        {
            _attackZoneTrigger.enabled = false;
            _enemyStateMachine.enabled = false;
            _enemyRotateToPlayer.enabled = false;
            _agent.enabled = false;
        }
    }
}
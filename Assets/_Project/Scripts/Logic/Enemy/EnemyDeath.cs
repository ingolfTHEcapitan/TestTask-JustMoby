using System;
using System.Collections;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Logic.Enemy
{
    public class EnemyDeath: MonoBehaviour
    {
        public event Action<EnemyDeath> OnDied;

        [SerializeField] private Health _health;
        [Header("Components To Disable On Death")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EnemyStateMachine _enemyStateMachine;
        [SerializeField] private EnemyRotateToPlayer _enemyRotateToPlayer;

        private bool _isDead;
        private EnemyConfig _config;

        [Inject]
        public void Construct(EnemyConfig config) => 
            _config = config;

        public void Initialize() => 
            _health.OnZeroHealth += EnemyDie;

        private void OnDestroy() => 
            _health.OnZeroHealth -= EnemyDie;
        
        public void KillEnemy() => 
            _health.TakeDamage(_health.MaxHealth);

        private void EnemyDie()
        {
            if (!_isDead)
                Die();
        }

        private void Die()
        {
            _isDead = true;
            DisableEnemyComponents();
            StartCoroutine(DestroyTimer(_config.DestroyDelay));
        }

        private IEnumerator DestroyTimer(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
            OnDied?.Invoke(this);
        }

        private void DisableEnemyComponents()
        {
            _enemyStateMachine.enabled = false;
            _enemyRotateToPlayer.enabled = false;
            _agent.enabled = false;
        }
    }
}
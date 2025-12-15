using System;
using System.Collections;
using _Project.Scripts.Logic.Common;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Logic.Enemy
{
    public class EnemyDeath: MonoBehaviour
    {
        public event Action<EnemyDeath> OnDied;

        [SerializeField] private Health _health;
        [SerializeField] private float _destroyDelay = 1.5f;
        [Header("Components To Disable On Death")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EnemyStateMachine _enemyStateMachine;
        [SerializeField] private EnemyRotateToPlayer _enemyRotateToPlayer;

        private bool _isDead;

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
            _enemyStateMachine.enabled = false;
            _enemyRotateToPlayer.enabled = false;
            _agent.enabled = false;
        }
    }
}
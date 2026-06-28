using System;
using System.Collections;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.Effects;
using _Project.Scripts.UI.Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Logic.Enemy
{
    public class EnemyDeath: MonoBehaviour
    {
        public event Action<EnemyDeath> OnDied;

        [SerializeField] private Health _health;
        [SerializeField] private HealthBarView _healthBarView;
        [SerializeField] private DissolveShader _dissolveShader;

        [Header("Components To Disable On Death")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EnemyStateMachine _enemyStateMachine;
        [SerializeField] private EnemyRotateToPlayer _enemyRotateToPlayer;

        private bool _isDead;
        private EnemyConfig _config;
        private WaitForSeconds _waitForSeconds;
        private EffectsService _effectsService;

        public bool IsForcedKilling { get; private set; }

        [Inject]
        private void Construct(EnemyConfig config, EffectsService effectsService)
        {
            _config = config;
            _effectsService = effectsService;
        }

        public void Initialize()
        {
            _health.OnZeroHealth += EnemyDie;
            _waitForSeconds = new WaitForSeconds(_config.DestroyDelay);
        }

        private void OnDestroy() => 
            _health.OnZeroHealth -= EnemyDie;
        
        [UsedImplicitly]
        public async void OnDeathPose()
        {
            _dissolveShader.PlayDissolveFx();
            await _effectsService.PlayEnemyDeathFx(transform.position, transform);
        }
        
        public void KillEnemy()
        {
            _health.TakeDamage(_health.MaxHealth);
            IsForcedKilling = true;
        }

        private void EnemyDie()
        {
            if (!_isDead)
                Die();
        }

        private void Die()
        {
            _isDead = true;
            _healthBarView.Hide();
            DisableEnemyComponents();
            StartCoroutine(DestroyTimer());
        }

        private IEnumerator DestroyTimer()
        { 
            yield return _waitForSeconds;
            OnDied?.Invoke(this);
            Destroy(gameObject);
        }

        private void DisableEnemyComponents()
        {
            _enemyStateMachine.enabled = false;
            _enemyRotateToPlayer.enabled = false;
            _agent.enabled = false;
        }
    }
}
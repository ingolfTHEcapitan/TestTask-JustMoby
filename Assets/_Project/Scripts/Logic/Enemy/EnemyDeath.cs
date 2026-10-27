using System;
using System.Collections;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Common.Health;
using _Project.Scripts.Services;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Common.HealthBar;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Logic.Enemy
{
    public class EnemyDeath : MonoBehaviour
    {
        public event Action<EnemyDeath> OnDied;

        [SerializeField] private Health _health;
        [SerializeField] private HealthBarView _healthBarView;
        [SerializeField] private DissolveShader _dissolveShader;

        [Header("Components To Disable On Death")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EnemyStateMachine _enemyStateMachine;
        [SerializeField] private EnemyRotateToPlayer _enemyRotateToPlayer;
        [SerializeField] private CapsuleCollider _capsuleCollider;

        private bool _isDead;
        private bool _isForcedKilling;

        private EnemyConfig _config;
        private EffectsService _effectsService;
        private GameStatistics _statistics;
        private ScoreService _scoreService;

        [Inject]
        private void Construct(EnemyConfig config, EffectsService effectsService, 
            GameStatistics statistics, ScoreService scoreService)
        {
            _config = config;
            _effectsService = effectsService;
            _statistics = statistics;
            _scoreService = scoreService;
        }

        public void Initialize() => 
            _health.OnZeroHealth += EnemyDie;

        private void OnDestroy() => 
            _health.OnZeroHealth -= EnemyDie;
        
        [UsedImplicitly]
        public void OnDeathPose()
        {
            _dissolveShader.PlayDissolveFx();
            _effectsService.PlayEnemyDeathFxAsync(transform.position, transform).Forget(Debug.LogError);
        }
        
        public void KillEnemy()
        {
            _health.TakeDamage(_health.MaxHealth);
            _isForcedKilling = true;
        }

        private void EnemyDie()
        {
            if (!_isDead)
                DieAsync().Forget(Debug.LogError);
        }

        private async UniTask DieAsync()
        {
            _isDead = true;
            DisableEnemyComponents();
            await _healthBarView.HideAsync();
            
            if (!_isForcedKilling)
            {
                _statistics.RecordEnemyKilled();
                _scoreService.Add();
            }

            await DestroyAsync();
        }

        private async UniTask DestroyAsync()
        { 
            bool isCancelled = await UniTask.WaitForSeconds(_config.DestroyDelay, cancellationToken: destroyCancellationToken)
                .SuppressCancellationThrow();
            
            if (isCancelled)
                return;
            
            OnDied?.Invoke(this);
            Destroy(gameObject);
        }

        private void DisableEnemyComponents()
        {
            _enemyStateMachine.enabled = false;
            _enemyRotateToPlayer.enabled = false;
            _agent.enabled = false;
            _capsuleCollider.enabled = false;
        }
    }
}
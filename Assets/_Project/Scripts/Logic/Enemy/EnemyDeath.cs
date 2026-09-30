using System;
using System.Collections;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.Effects;
using _Project.Scripts.Services.Score;
using _Project.Scripts.Services.Statistics;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private CapsuleCollider _capsuleCollider;

        private bool _isDead;
        private bool _isForcedKilling;
        private WaitForSeconds _waitForSeconds;

        private EnemyConfig _config;
        private IEffectsService _effectsService;
        private IGameStatistics _statistics;
        private IScoreService _scoreService;

        [Inject]
        private void Construct(EnemyConfig config, IEffectsService effectsService, 
            IGameStatistics statistics, IScoreService scoreService)
        {
            _config = config;
            _effectsService = effectsService;
            _statistics = statistics;
            _scoreService = scoreService;
        }

        public void Initialize()
        {
            _health.OnZeroHealth += EnemyDie;
            _waitForSeconds = new WaitForSeconds(_config.DestroyDelay);
        }

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
            StartCoroutine(DestroyTimer());

            if (!_isForcedKilling)
            {
                _statistics.RecordEnemyKilled();
                _scoreService.Add();
            }
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
            _capsuleCollider.enabled = false;
        }
    }
}
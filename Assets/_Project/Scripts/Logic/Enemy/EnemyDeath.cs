using System;
using System.Collections;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.Effects;
using _Project.Scripts.Services.Statistics;
using _Project.Scripts.Services.UpgradePoints;
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
        private IUpgradePointsService _upgradePoints;

        [Inject]
        private void Construct(EnemyConfig config, IEffectsService effectsService, 
            IGameStatistics statistics, IUpgradePointsService upgradePoints)
        {
            _config = config;
            _effectsService = effectsService;
            _statistics = statistics;
            _upgradePoints = upgradePoints;
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
            await _effectsService.PlayEnemyDeathFxAsync(transform.position, transform);
        }
        
        public void KillEnemy()
        {
            _health.TakeDamage(_health.MaxHealth);
            _isForcedKilling = true;
        }

        private async void EnemyDie()
        {
            if (!_isDead)
                await DieAsync();
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
                await _upgradePoints.AddPointAsync();
                
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
using System;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Enemy.Components.SatteMachine;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Logic.Enemy.Components
{
    public class EnemyDeath : MonoBehaviour
    {
        public event Action OnDied;

        [SerializeField] private Health _health;
        
        [Header("Components To Disable On Death")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EnemyStateMachine _enemyStateMachine;
        [SerializeField] private EnemyRotateToPlayer _enemyRotateToPlayer;
        [SerializeField] private CapsuleCollider _capsuleCollider;

        private bool _isDead;
        private bool _isForcedKilling;

        private EnemyConfig _config;
        private GameStatistics _statistics;
        private ScoreService _scoreService;

        [Inject]
        private void Construct(EnemyConfig config, GameStatistics statistics, ScoreService scoreService)
        {
            _config = config;
            _statistics = statistics;
            _scoreService = scoreService;
        }

        public void Initialize() => 
            _health.OnZeroHealth += EnemyDie;

        private void OnDestroy() => 
            _health.OnZeroHealth -= EnemyDie;
        
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
            
            OnDied?.Invoke();
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
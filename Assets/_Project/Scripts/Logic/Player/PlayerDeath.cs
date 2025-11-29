using System;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Statistics;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player
{
    public class PlayerDeath : MonoBehaviour
    {
        private const float HealPercent = 0.5f;
        
        public event Action OnDied;

        [SerializeField] private Health _health;
        [SerializeField] private PlayerCameraLook _playerCameraLook;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private Weapon.Weapon _weapon;
        
        private IAnalyticsService _analyticsService;
        private IGameStatistics _statistics;
        
        public bool IsDead { get; private set; }
        
        [Inject]
        public void Construct(IAnalyticsService analyticsService, IGameStatistics statistics)
        {
            _analyticsService = analyticsService;
            _statistics = statistics;
        }
        
        public void Initialize() => 
            _health.OnHealthChanged += OnOnHealthChanged;

        private void OnDestroy() => 
            _health.OnHealthChanged -= OnOnHealthChanged;

        public void Revive()
        {
            IsDead = false;
            _statistics.RecordRevive();
            _health.TakeHeal(_health.MaxHealth * HealPercent);
            EnablePlayerComponents(true);
            _analyticsService.LogPlayerRevive(_statistics.ReviveCount);
        }
        
        private void OnOnHealthChanged()
        {
            if (!IsDead && _health.CurrentHealth <= 0) 
                Die();
        }

        private void Die()
        {
            IsDead = true;
            EnablePlayerComponents(false);
            OnDied?.Invoke();
            _analyticsService.LogGameEnd(_statistics.ShotsFired, _statistics.EnemiesKilled);
        }
        
        private void EnablePlayerComponents(bool enable)
        {
            _playerCameraLook.enabled = enable;
            _playerMovement.enabled = enable;
            _weapon.enabled = enable;
        }
    }
}
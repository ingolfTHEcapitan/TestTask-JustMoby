using System;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Sound;
using _Project.Scripts.Services.Statistics;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player
{
    public class PlayerDeath : MonoBehaviour
    {
        private const float HealPercent = 0.5f;
        private const float DeathSoundDelay = 0.2f;
        public event Action OnDied;

        [SerializeField] private Health _health;
        [SerializeField] private PlayerCameraLook _playerCameraLook;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private Weapon.Weapon _weapon;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _deathSound;
        
        private IAnalyticsService _analyticsService;
        private IGameStatistics _statistics;
        private IAudioService _audioService;

        public bool IsDead { get; private set; }
        
        [Inject]
        private void Construct(IAnalyticsService analyticsService, IGameStatistics statistics, IAudioService audioService)
        {
            _analyticsService = analyticsService;
            _statistics = statistics;
            _audioService = audioService;
        }
        
        public void Initialize() => 
            _health.OnZeroHealth += PlayerDie;

        private void OnDestroy() => 
            _health.OnZeroHealth -= PlayerDie;

        public void Revive()
        {
            IsDead = false;
            _statistics.RecordRevive();
            _health.TakeHeal(_health.MaxHealth * HealPercent);
            EnablePlayerComponents(true);
            _analyticsService.LogPlayerRevive(_statistics.ReviveCount);
        }
        
        private void PlayerDie()
        {
            if (!IsDead) 
                Die();
        }

        private void Die()
        {
            IsDead = true;
            EnablePlayerComponents(false);
            OnDied?.Invoke();
            _audioService.PlayDelayed(_deathSound, _audioSource, DeathSoundDelay);
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
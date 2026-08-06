using System;
using System.Collections.Generic;
using _Project.Scripts.Services.Sound;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Common
{
    public class Health: MonoBehaviour, IHealth
    {
        public event Action OnHealthChanged;
        public event Action OnZeroHealth;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _healSound;
        [SerializeField] private List<AudioClip> _hitSounds;

        private bool _isDead;
        private IAudioService _audioService;

        public float CurrentHealth {get; private set;}
        public float MaxHealth { get; private set; }

        [Inject]
        private void Construct(IAudioService audioService) => 
            _audioService = audioService;

        public void Initialize(float maxHealth)
        {
            SetMaxHealth(maxHealth);
            CurrentHealth = MaxHealth;
        }

        public void SetMaxHealth(float maxHealth)
        {
            MaxHealth = Mathf.Max(CurrentHealth, maxHealth);
            OnHealthChanged?.Invoke();
        }

        public void TakeDamage(float damage)
        {
            if (CurrentHealth <= 0)
                return;
            
            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
            OnHealthChanged?.Invoke();
            _audioService.PlayOneShotRandom(_hitSounds, _audioSource);

            if (CurrentHealth <= 0) 
                OnZeroHealth?.Invoke();
        }

        public void TakeHeal(float amount)
        {
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke();
            
            if (_healSound != null)
                _audioService.PlayOneShot(_healSound, _audioSource);
        }
    }
}
using System;
using _Project._Scripts.Enemy;
using _Project._Scripts.Logic;
using _Project._Scripts.Logic.StatSystem;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerHealth: MonoBehaviour, IHealth
    {
        private PlayerStatsSystem _playerStatsSystem;
        private float _currentHealth;

        public float MaxHealth => _playerStatsSystem.GetStatValue(StatName.Health);

        public float CurrentHealth
        {
            get => MaxHealth;
            private set => _currentHealth = value;
        }

        public event Action OnHealthChanged;
        
        public void Construct(PlayerStatsSystem playerStatsSystem) => 
            _playerStatsSystem = playerStatsSystem;
        
        public void Initialize()
        {
            CurrentHealth = MaxHealth;
            _playerStatsSystem.OnStatsChanged += InvokeOnHealthChanged;
        }
        
        public void TakeDamage(float damage)
        {
            if (CurrentHealth <= 0)
                return;
            
            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
            InvokeOnHealthChanged();
        }

        private void InvokeOnHealthChanged() => 
            OnHealthChanged?.Invoke();
    }
}
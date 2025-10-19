using System;
using _Project._Scripts.Enemy;
using _Project._Scripts.Logic;
using _Project._Scripts.Logic.PlayerStats;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerHealth: MonoBehaviour, IHealth
    {
        public event Action OnHealthChanged;
        
        private PlayerStatsModel _playerStatsModel;
        private float _currentHealth;

        public float MaxHealth => 
            _playerStatsModel.GetStatValue(StatName.Health);

        public float CurrentHealth
        {
            get => MaxHealth;
            private set => _currentHealth = value;
        }
        
        public void Construct(PlayerStatsModel playerStatsModel) => 
            _playerStatsModel = playerStatsModel;

        private void OnDestroy() => 
            _playerStatsModel.OnStatsChanged -= InvokeOnHealthChanged;

        public void Initialize()
        {
            CurrentHealth = MaxHealth;
            _playerStatsModel.OnStatsChanged += InvokeOnHealthChanged;
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
using System;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.Logic.Common
{
    public class Health: MonoBehaviour, IHealth
    {
        public event Action OnHealthChanged;
        
        public float CurrentHealth {get; private set;}
        public float MaxHealth { get; private set; }
        
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
        }

        public void TakeHeal(float amount)
        {
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke();
        }
    }
}
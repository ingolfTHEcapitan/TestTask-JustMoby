using System;
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
            MaxHealth = maxHealth;
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (CurrentHealth <= 0)
                return;
            
            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
            InvokeOnHealthChanged();
        }

        public void InvokeOnHealthChanged() => 
            OnHealthChanged?.Invoke();
    }
}
using System;
using _Project._Scripts.Logic;
using UnityEngine;

namespace _Project._Scripts.Enemy
{
    public class EnemyHealth : MonoBehaviour, IHealth
    {
        [SerializeField] private float _maxHealth = 100f;
        
        public event Action OnHealthChanged;
        
        public float CurrentHealth {get; private set;}

        public float MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }

        private void Awake() => 
            CurrentHealth = MaxHealth;

        public void TakeDamage(float damage)
        {
            if (CurrentHealth <= 0)
                return;
            
            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
            OnHealthChanged?.Invoke();
        }
    }
}



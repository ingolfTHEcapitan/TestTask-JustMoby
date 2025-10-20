using System;
using _Project._Scripts.Logic;
using _Project._Scripts.Logic.PlayerStats;
using UnityEngine;

namespace _Project._Scripts.Enemy
{
    public class EnemyHealth : MonoBehaviour, IHealth
    {
        public event Action OnHealthChanged;
        
        private PlayerStatsModel _playerStatsModel;
        
        public float CurrentHealth {get; private set;}
        public float MaxHealth { get; private set; }

        public void Construct(PlayerStatsModel playerStatsModel) => 
            _playerStatsModel = playerStatsModel;

        public void Initialize()
        {
            MaxHealth = CalculateMaxHealth();
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (CurrentHealth <= 0)
                return;
            
            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
            OnHealthChanged?.Invoke();
        }

        private float CalculateMaxHealth()
        {
            PlayerStatData damageStat = _playerStatsModel.Stats[StatName.Damage];

            int minShotsToKill = 1;
            int maxShotsToKill = 10;
            
            int randomShootsCount = UnityEngine.Random.Range(minShotsToKill, maxShotsToKill + 1);
            float maxHealth = damageStat.BaseValue * randomShootsCount;
            return maxHealth;
        }
    }
}



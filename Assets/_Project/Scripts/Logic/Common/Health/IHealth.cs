using System;

namespace _Project.Scripts.Logic.Common.Health
{
    public interface IHealth
    {
        event Action OnHealthChanged;
        event Action OnTakeDamage;
        event Action OnTakeHeal;
        float MaxHealth { get; }
        float CurrentHealth { get; }
        float NormalizedHealth { get; }
        void TakeDamage(float damage);
        void SetMaxHealth(float maxHealth);
        void TakeHeal(float amount);
    }
}
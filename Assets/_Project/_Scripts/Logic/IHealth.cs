using System;

namespace _Project._Scripts.Logic
{
    public interface IHealth
    {
        event Action OnHealthChanged;
        float MaxHealth { get; }
        float CurrentHealth { get; }
        void TakeDamage(float damage);
    }
}
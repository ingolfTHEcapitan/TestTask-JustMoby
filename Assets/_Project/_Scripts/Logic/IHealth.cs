using System;

namespace _Project._Scripts.Logic
{
    public interface IHealth
    {
        float MaxHealth { get; }
        float CurrentHealth { get; }
        event Action OnHealthChanged;
        void TakeDamage(float damage);
    }
}
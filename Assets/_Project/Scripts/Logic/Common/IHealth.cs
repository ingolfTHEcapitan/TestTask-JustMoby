using System;

namespace _Project.Scripts.Logic.Common
{
    public interface IHealth
    {
        event Action OnHealthChanged;
        float MaxHealth { get; }
        float CurrentHealth { get; }
        void TakeDamage(float damage);
    }
}
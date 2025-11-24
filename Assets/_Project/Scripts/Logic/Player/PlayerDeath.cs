using System;
using _Project.Scripts.Logic.Common;
using UnityEngine;

namespace _Project.Scripts.Logic.Player
{
    public class PlayerDeath : MonoBehaviour
    {
        private const float HealPercent = 0.5f;
        
        public event Action OnDied;

        [SerializeField] private Health _health;
        [SerializeField] private PlayerCameraLook _playerCameraLook;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private Weapon.Weapon _weapon;
        
        public bool IsDead { get; private set; }
        
        public void Initialize() => 
            _health.OnHealthChanged += OnOnHealthChanged;

        private void OnDestroy() => 
            _health.OnHealthChanged -= OnOnHealthChanged;

        public void Revive()
        {
            IsDead = false;
            _health.TakeHeal(_health.MaxHealth * HealPercent);
            EnablePlayerComponents(true);
        }
        
        private void OnOnHealthChanged()
        {
            if (!IsDead && _health.CurrentHealth <= 0) 
                Die();
        }

        private void Die()
        {
            IsDead = true;
            EnablePlayerComponents(false);
            OnDied?.Invoke();
        }
        
        private void EnablePlayerComponents(bool enable)
        {
            _playerCameraLook.enabled = enable;
            _playerMovement.enabled = enable;
            _weapon.enabled = enable;
        }
    }
}
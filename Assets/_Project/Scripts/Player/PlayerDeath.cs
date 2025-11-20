using System;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Weapon;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Player
{
    public class PlayerDeath : MonoBehaviour
    {
        private const float HealPercent = 0.5f;
        
        public event Action OnDied;

        [SerializeField] private Health _health;
        [SerializeField] private PlayerCameraLook _playerCameraLook;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private Weapon _weapon;
        
        private bool _isDeath;
        
        public void Initialize() => 
            _health.OnHealthChanged += OnOnHealthChanged;

        private void OnDestroy() => 
            _health.OnHealthChanged -= OnOnHealthChanged;

        public void Revive()
        {
            _isDeath = false;
            _health.TakeHeal(_health.MaxHealth * HealPercent);
            EnablePlayerComponents(true);
        }
        
        private void OnOnHealthChanged()
        {
            if (!_isDeath && _health.CurrentHealth <= 0) 
                Die();
        }

        private void Die()
        {
            _isDeath = true;
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
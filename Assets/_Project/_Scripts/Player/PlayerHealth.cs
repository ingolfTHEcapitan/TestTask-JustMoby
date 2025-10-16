using System;
using _Project._Scripts.Player.StatSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Player
{
    public class PlayerHealth: MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Slider _maxHealthSlider;
        public event Action OnHealthChanged;
        public event Action OnDeath;
        
        private PlayerStatsSystem _playerStatsSystem;
        private float _currentHealth;
        private float MaxHealth => _playerStatsSystem.GetStatValue(StatName.Health);

        public void Construct(PlayerStatsSystem playerStatsSystem)
        {
            _playerStatsSystem = playerStatsSystem;
        }
        
        public void Initialize()
        {
            _currentHealth = MaxHealth;
            OnHealthChanged +=  UpdateView;
            _playerStatsSystem.OnStatsChanged += UpdateView;
        }
        
        private void Start()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            _healthSlider.value = _currentHealth / MaxHealth;
            _healthText.text = $"{_currentHealth}/{MaxHealth}";
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TakeDamage(10);
            }
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= Mathf.Max(damage, 0f);
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);
            OnHealthChanged?.Invoke();

            if (_currentHealth <=0) 
                OnDeath?.Invoke();
        }
    }
}
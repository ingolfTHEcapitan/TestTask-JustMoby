using System;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace _Project._Scripts.Player
{
    public class PlayerHealth: MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        
        public event Action OnHealthChanged;
        public event Action OnDeath;
        
        private float _currentHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged +=  UpdateView;
        }
        
        private void Start()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            _healthSlider.value = _currentHealth / _maxHealth;
            _healthText.text = $"{_currentHealth}/{_maxHealth}";
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
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
            OnHealthChanged?.Invoke();

            if (_currentHealth <=0) 
                OnDeath?.Invoke();
        }
        
        
    }
}
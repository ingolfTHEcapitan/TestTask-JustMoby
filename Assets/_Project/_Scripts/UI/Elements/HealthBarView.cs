using _Project._Scripts.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI.Elements
{
    public class HealthBarView: MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        
        private IHealth _health;

        public void Construct(IHealth health)
        {
            _health = health;
        }
        
        public void Initialize()
        {
            _health.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar();
        }
        
        private void UpdateHealthBar()
        {
            _healthSlider.value = _health.CurrentHealth / _health.MaxHealth;
            _healthText.SetText($"{_health.CurrentHealth}/{_health.MaxHealth}");
        }
    }
}
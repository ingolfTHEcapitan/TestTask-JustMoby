using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Common.HealthBar
{
    public class HealthBarView : MonoBehaviour
    {
        public event Action OnHealthBarDestroy;

        [field:SerializeField] public float SliderValuePerSecond { get; private set; } = 0.5f;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;

        public float HealthSliderValue => _healthSlider.value;
        
        private void OnDestroy() => 
            OnHealthBarDestroy?.Invoke();

        public void UpdateHealthSliderValue(float value) => 
            _healthSlider.value = value;

        public void UpdateHealthText(float currentHealth, float maxHealth) => 
            _healthText.text = $"{currentHealth}/{maxHealth}";

        public void Hide() => 
            gameObject.SetActive(false);
    }
}
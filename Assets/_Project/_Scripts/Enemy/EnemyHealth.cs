using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        
        public event Action OnHealthChanged;
        
        public float Current {get; private set;}

        private void Awake()
        {
            Current = _maxHealth;
            OnHealthChanged +=  UpdateView;
        }
        
        private void Start()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            _healthSlider.value = Current / _maxHealth;
            _healthText.text = $"{Current}/{_maxHealth}";
        }
        
        public void TakeDamage(float damage)
        {
            Current -= Mathf.Max(damage, 0f);
            Current = Mathf.Clamp(Current, 0f, _maxHealth);
            OnHealthChanged?.Invoke();
        }
    }
}



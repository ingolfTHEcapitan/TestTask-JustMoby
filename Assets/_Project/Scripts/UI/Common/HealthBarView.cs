using System.Collections;
using _Project.Scripts.Logic.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Common
{
    public class HealthBarView: MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private float _lerpSpeed = 1f;
        [SerializeField] private float _lerpTime = 2f;
        
        private IHealth _health;
        private Coroutine _lerpCoroutine;

        private float CurrentValue => _health.CurrentHealth / _health.MaxHealth;

        public void Construct(IHealth health) => 
            _health = health;

        public void Initialize()
        {
            _health.OnHealthChanged += UpdateHealthBar;
            InitHealthBar();
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.OnHealthChanged -= UpdateHealthBar;
        }

        public void Hide() => 
            gameObject.SetActive(false);

        private void InitHealthBar()
        {
            _healthText.text = $"{_health.CurrentHealth}/{_health.MaxHealth}";
            _healthSlider.value = CurrentValue;
        }

        private void UpdateHealthBar()
        {
            _healthText.text = $"{_health.CurrentHealth}/{_health.MaxHealth}";

            if (_lerpCoroutine != null)
                StopCoroutine(_lerpCoroutine);
            
            _lerpCoroutine = StartCoroutine(LerpHealthSliderRoutine());
        }

        private IEnumerator LerpHealthSliderRoutine()
        {
            float elapsedTime = 0;

            while (elapsedTime < _lerpTime)
            {
                if (_healthSlider.value != CurrentValue) 
                    _healthSlider.value = Mathf.Lerp(_healthSlider.value, CurrentValue, elapsedTime);
                
                elapsedTime += Time.deltaTime * _lerpSpeed;
                
                yield return null;
            }
        }
    }
}
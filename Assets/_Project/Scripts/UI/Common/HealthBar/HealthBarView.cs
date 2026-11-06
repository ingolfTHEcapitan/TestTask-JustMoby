using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Common.HealthBar
{
    public class HealthBarView : MonoBehaviour
    {
        public event Action OnHealthBarDestroy;

        [SerializeField] private float _sliderValuePerSecond = 0.5f;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        
        private UniTask _currentLerpTask;
        private CancellationTokenSource _cts;
        
        private void OnDestroy()
        {
            OnHealthBarDestroy?.Invoke();
           
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public void Initialize(float currentHealth, float maxHealth)
        {
            float normalizedHealth = currentHealth / maxHealth;
           UpdateHealthText(currentHealth, maxHealth);
           _healthSlider.value = normalizedHealth;
        }

        public async UniTask HideAsync()
        {
            if (_currentLerpTask.Status == UniTaskStatus.Pending) 
                await _currentLerpTask;
            
            gameObject.SetActive(false);
        }
        
        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            float normalizedHealth = currentHealth / maxHealth;
            
            UpdateHealthText(currentHealth, maxHealth);
            _currentLerpTask = LerpHealthSliderAsync(_cts.Token, normalizedHealth);
        }
        
        private async UniTask LerpHealthSliderAsync(CancellationToken token, float normalizedHealth)
        {
            float startValue = _healthSlider.value;
            float targetValue = normalizedHealth;
            float elapsedTime = 0;
            
            float distance = Mathf.Abs(targetValue - startValue);
            float duration = distance / _sliderValuePerSecond;
            
            
            if (Mathf.Approximately(startValue, targetValue))
                return;
            
            while (!Mathf.Approximately(_healthSlider.value , targetValue))
            {
                if(token.IsCancellationRequested)
                    break;
                
                elapsedTime += Time.deltaTime;
                float time = Mathf.Clamp01(elapsedTime / duration);
                
                _healthSlider.value = Mathf.Lerp(startValue, targetValue, time);
                
                await UniTask.Yield(token).SuppressCancellationThrow();
            }
        }

        private void UpdateHealthText(float currentHealth, float maxHealth) => 
            _healthText.text = $"{currentHealth}/{maxHealth}";
    }
}
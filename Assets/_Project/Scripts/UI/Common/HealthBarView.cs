using System.Threading;
using System.Threading.Tasks;
using _Project.Scripts.Logic.Common;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Common
{
    public class HealthBarView: MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private float _sliderValuePerSecond = 0.5f;
        
        private IHealth _health;
        private UniTask _currentLerpTask;
        private CancellationTokenSource _cts;

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
            _cts?.Cancel();
            _cts?.Dispose();
            
            if (_health != null)
                _health.OnHealthChanged -= UpdateHealthBar;
        }

        public async UniTask Hide()
        {
            Task taskToWait = _currentLerpTask.AsTask();
            if (taskToWait != null && !taskToWait.IsCompleted)
            {
                await taskToWait;
            }
            gameObject.SetActive(false);
        }

        private void InitHealthBar()
        {
            _healthText.text = $"{_health.CurrentHealth}/{_health.MaxHealth}";
            _healthSlider.value = CurrentValue;
        }

        private void UpdateHealthBar()
        {
            _healthText.text = $"{_health.CurrentHealth}/{_health.MaxHealth}";

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            
            _currentLerpTask = LerpHealthSliderRoutine(_cts.Token);
        }

        private async UniTask LerpHealthSliderRoutine(CancellationToken token)
        {
            float startValue = _healthSlider.value;
            float targetValue = CurrentValue;
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
                
                await UniTask.Yield(token);
            }
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.Sound;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Common
{
    public class HealthBarView: MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private float _sliderValuePerSecond = 0.5f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _takeHealSound;
        [SerializeField] private List<AudioClip> _takeDamageSounds;
        
        private IHealth _health;
        private IAudioService _audioService;
        private UniTask _currentLerpTask;
        private CancellationTokenSource _cts;

        private float CurrentValue => _health.CurrentHealth / _health.MaxHealth;

        [Inject]
        public void Construct(IAudioService audioService) => 
            _audioService = audioService;

        public void Construct(IHealth health) => 
            _health = health;

        public void Initialize()
        {
            _health.OnHealthChanged += UpdateHealthBar;
            _health.OnTakeDamage += PlayHitSound;
            _health.OnTakeHeal += PlayHealSound;
            InitHealthBar();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            
            if (_health != null)
                _health.OnHealthChanged -= UpdateHealthBar;
        }

        public async UniTask HideAsync()
        {
            if (_currentLerpTask.Status == UniTaskStatus.Pending) 
                await _currentLerpTask;
            
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
            
            _currentLerpTask = LerpHealthSliderAsync(_cts.Token);
        }

        private async UniTask LerpHealthSliderAsync(CancellationToken token)
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
        
        private void PlayHitSound() => 
            _audioService.PlayOneShotRandom(_takeDamageSounds, _audioSource);
        
        private void PlayHealSound()
        {
            if (_takeHealSound != null)
                _audioService.PlayOneShot(_takeHealSound, _audioSource);
        }
    }
}
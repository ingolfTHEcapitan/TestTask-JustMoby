using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Common.Health;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Common
{
    public class HealthBarView : MonoBehaviour
    {
        //V
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private float _sliderValuePerSecond = 0.5f;
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _takeHealSound;
        [SerializeField] private List<AudioClip> _takeDamageSounds;
        
        //M
        private IHealth _health;
        private AudioService _audioService;
        //P
        private UniTask _currentLerpTask;
        private CancellationTokenSource _cts;

        private float CurrentValue => _health.CurrentHealth / _health.MaxHealth;

        [Inject]
        private void Construct(AudioService audioService) => 
            _audioService = audioService;

        //M
        public void Construct(IHealth health) => 
            _health = health;

        //M V P
        public void Initialize()
        {
            //M P
            _health.OnHealthChanged += UpdateHealthBar;
            _health.OnTakeDamage += PlayHitSound;
            _health.OnTakeHeal += PlayHealSound;
            //V
            InitHealthBar();
        }

        // M P
        private void OnDestroy()
        {
            //M
            if (_health != null)
            {
                _health.OnHealthChanged -= UpdateHealthBar;
                _health.OnTakeDamage -= PlayHitSound;
                _health.OnTakeHeal -= PlayHealSound;
            }

            //P
            _cts?.Cancel();
            _cts?.Dispose();
        }

        //P
        public async UniTask HideAsync()
        {
            if (_currentLerpTask.Status == UniTaskStatus.Pending) 
                await _currentLerpTask;
            
            //V
            gameObject.SetActive(false);
        }

        //V
        private void InitHealthBar()
        {
            _healthText.text = $"{_health.CurrentHealth}/{_health.MaxHealth}";
            _healthSlider.value = CurrentValue;
        }
        
        //V P
        private void UpdateHealthBar()
        {
            //V
            _healthText.text = $"{_health.CurrentHealth}/{_health.MaxHealth}";

            //P
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            
            _currentLerpTask = LerpHealthSliderAsync(_cts.Token);
        }

        //P
        private async UniTask LerpHealthSliderAsync(CancellationToken token)
        {
            //V
            float startValue = _healthSlider.value;
            //M
            float targetValue = CurrentValue;
            //P
            float elapsedTime = 0;
            //P
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
        
        //V
        private void PlayHitSound() => 
            _audioService.PlayOneShotRandom(_takeDamageSounds, _audioSource);
        
        //V
        private void PlayHealSound()
        {
            if (_takeHealSound != null)
                _audioService.PlayOneShot(_takeHealSound, _audioSource);
        }
    }
}
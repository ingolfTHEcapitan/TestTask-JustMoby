using System;
using System.Collections.Generic;
using _Project.Scripts.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Common.HealthBar
{
    public class HealthBarView : MonoBehaviour
    {
        public event Action OnHealthBarDestroy;

        [field:SerializeField] public float SliderValuePerSecond { get; private set; } = 0.5f;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _takeHealSound;
        [SerializeField] private List<AudioClip> _takeDamageSounds;

        public float HealthSliderValue => _healthSlider.value;
        
        private AudioService _audioService;
        
        [Inject]
        private void Construct(AudioService audioService) => 
            _audioService = audioService;
        
        private void OnDestroy() => 
            OnHealthBarDestroy?.Invoke();

        public void UpdateHealthSliderValue(float value) => 
            _healthSlider.value = value;

        public void UpdateHealthText(float currentHealth, float maxHealth) => 
            _healthText.text = $"{currentHealth}/{maxHealth}";

        public void Hide() => 
            gameObject.SetActive(false);
        
        public void PlayHitSound() => 
            _audioService.PlayOneShotRandom(_takeDamageSounds, _audioSource);
        
        public void PlayHealSound()
        {
            if (_takeHealSound != null)
                _audioService.PlayOneShot(_takeHealSound, _audioSource);
        }
    }
}
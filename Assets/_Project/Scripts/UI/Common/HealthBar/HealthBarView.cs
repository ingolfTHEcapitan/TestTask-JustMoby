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
        //V+
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private float _sliderValuePerSecond = 0.5f;
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _takeHealSound;
        [SerializeField] private List<AudioClip> _takeDamageSounds;
        
        //V+
        private AudioService _audioService;
        
        //V
        [Inject]
        private void Construct(AudioService audioService) => 
            _audioService = audioService;
        
        public void UpdateHealthSliderValue(float value) => 
            _healthSlider.value = value;

        public void UpdateHealthText(float currentHealth, float maxHealth) => 
            _healthText.text = $"{currentHealth}/{maxHealth}";

        public void Hide() => 
            gameObject.SetActive(false);
    }
}
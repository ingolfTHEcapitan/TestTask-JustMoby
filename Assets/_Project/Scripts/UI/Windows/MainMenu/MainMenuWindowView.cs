using System;
using _Project.Scripts.Services.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.MainMenu
{
    public class MainMenuWindowView: MonoBehaviour
    {
        public event Action OnPlayButtonClicked;
        public event Action OnSettingsButtonClicked;
        public event Action OnShopButtonClicked;
        public event Action OnExitButtonClicked;
        
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _exitButton;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioClip _backgroundMusic;
        
        private IAudioService _audioService;
        
        [Inject] 
        private void Construct(IAudioService audioService) => 
            _audioService = audioService;
        
        public void Initialize()
        {
            _playButton.onClick.AddListener(InvokeOnPlayButtonClicked);
            _settingsButton.onClick.AddListener(InvokeOnSettingsButtonClicked);
            _shopButton.onClick.AddListener(InvokeOnShopButtonClicked);
            _exitButton.onClick.AddListener(InvokeOnExitButtonClicked);
        }
        
        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(InvokeOnPlayButtonClicked);
            _settingsButton.onClick.RemoveListener(InvokeOnSettingsButtonClicked);
            _shopButton.onClick.RemoveListener(InvokeOnShopButtonClicked);
            _exitButton.onClick.RemoveListener(InvokeOnExitButtonClicked);
            StopBackgroundMusic();
        }
        
        public void PlayBackgroundMusic() => 
            _audioService.Play(_backgroundMusic, _musicAudioSource);

        public void StopBackgroundMusic() => 
            _audioService.Stop(_musicAudioSource);

        private void InvokeOnPlayButtonClicked() => 
            OnPlayButtonClicked?.Invoke();

        private void InvokeOnSettingsButtonClicked() => 
            OnSettingsButtonClicked?.Invoke();

        private void InvokeOnShopButtonClicked() => 
            OnShopButtonClicked?.Invoke();
        
        private void InvokeOnExitButtonClicked() => 
            OnExitButtonClicked?.Invoke();
    }
}
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.Settings
{
    public class SettingsWindow: MonoBehaviour
    {
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;

        [Header("Sliders")]
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _effectsSlider;
        [SerializeField] private Slider _uiSlider;

        [Header("Audio")]
        [SerializeField] private AudioMixer _audioMixer;
        
        private IProgressService _progressService;
        private ISaveLoadService _saveService;

        private const string MasterVolumeName = "MasterVolume";
        private const string MusicVolumeName = "MusicVolume";
        private const string EffectsVolumeName = "EffectsVolume";
        private const string UIVolumeName = "UIVolume";

        private AudioSettingsData AudioSettingsData => _progressService.PlayerProgress.AudioSettingsData;

        [Inject]
        private void Construct(IProgressService progressService, 
            [Inject(Id = SaveType.Coordinator)] ISaveLoadService saveService)
        {
            _saveService = saveService;
            _progressService = progressService;
        }

        public void Initialize()
        {
            _closeButton.onClick.AddListener(Close);
            _applyButton.onClick.AddListener(ApplySettings);
            SubscribeSliderEvents();
            
            InitSliders();
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(Close);
            _applyButton.onClick.RemoveListener(ApplySettings);
            UnSubscribeSliderEvents();
        }

        private void InitSliders()
        {
            SetSliderValue(MasterVolumeName, _masterSlider, AudioSettingsData.MasterVolume);
            SetSliderValue(MusicVolumeName, _musicSlider, AudioSettingsData.MusicVolume);
            SetSliderValue(EffectsVolumeName, _effectsSlider, AudioSettingsData.EffectsVolume);
            SetSliderValue(UIVolumeName, _uiSlider, AudioSettingsData.UIVolume);
        }


        public void Open()
        {
            InitSliders();
            gameObject.SetActive(true);
            _windowAnimation.AnimateOpen();
        }

        private async void Close()
        {
            InitSliders();
            await _windowAnimation.AnimateClose();
            gameObject.SetActive(false);
        }

        private void SetSliderValue(string volumeName, Slider slider, float SavedValue)
        {
            slider.value = SavedValue;
            SetVolume(volumeName, SavedValue);
        }

        private void SetVolume(string volumeName, float volume)
        {
            float db;
            if (volume < 1e-06)
                db = -80;
            else
                db = Mathf.Log10(volume) * 20;
            
            _audioMixer.SetFloat(volumeName, db);
        }

        private async void ApplySettings()
        {
            AudioSettingsData.MasterVolume = _masterSlider.value;
            AudioSettingsData.MusicVolume = _musicSlider.value;
            AudioSettingsData.EffectsVolume = _effectsSlider.value;
            AudioSettingsData.UIVolume = _uiSlider.value;

            await _windowAnimation.AnimateClose();
            gameObject.SetActive(false);
            await _saveService.SaveProgressAsync(_progressService);
        }

        private void SubscribeSliderEvents()
        {
            _masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            _effectsSlider.onValueChanged.AddListener(OnEffectsVolumeChanged);
            _uiSlider.onValueChanged.AddListener(OnUIVolumeChanged);
        }
        
        private void UnSubscribeSliderEvents()
        {
            _masterSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            _musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            _effectsSlider.onValueChanged.RemoveListener(OnEffectsVolumeChanged);
            _uiSlider.onValueChanged.RemoveListener(OnUIVolumeChanged);
        }

        private void OnMasterVolumeChanged(float value) => 
            SetVolume(MasterVolumeName, value);
        private void OnMusicVolumeChanged(float value) => 
            SetVolume(MusicVolumeName, value);
        private void OnEffectsVolumeChanged(float value) => 
            SetVolume(EffectsVolumeName, value);
        private void OnUIVolumeChanged(float value) => 
            SetVolume(UIVolumeName, value);
    }
}
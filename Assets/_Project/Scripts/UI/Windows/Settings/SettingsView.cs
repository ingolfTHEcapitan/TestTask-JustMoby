using System;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.Settings
{
    public class SettingsView: MonoBehaviour, IWindow
    {
        public event Action OnCloseButtonClicked;
        public event Action OnApplyButtonClicked;
        public event Action OnOpen;
        
        public event Action<float> OnMasterVolumeChanged;
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnEffectsVolumeChanged;
        public event Action<float> OnUIVolumeChanged; 
        
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
        
        public float MasterVolume => _masterSlider.value;
        public float MusicVolume => _musicSlider.value;
        public float EffectsVolume => _effectsSlider.value;
        public float UIVolume => _uiSlider.value;

        public void Initialize()
        {
            _closeButton.onClick.AddListener(InvokeOnCloseButtonClicked);
            _applyButton.onClick.AddListener(InvokeOnApplyButtonClicked);
            SubscribeSliderEvents();
        }
        
        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(InvokeOnCloseButtonClicked);
            _applyButton.onClick.RemoveListener(InvokeOnApplyButtonClicked);
            UnSubscribeSliderEvents();
        }
        
        public void Open()
        {
            gameObject.SetActive(true);
            _windowAnimation.AnimateOpen();
            OnOpen?.Invoke();
        }

        public async UniTask CloseAsync()
        {
            await _windowAnimation.AnimateCloseAsync();
            gameObject.SetActive(false);
        }

        public void SetSlidersValues(float master, float music, float effects, float ui)
        {
            _masterSlider.SetValueWithoutNotify(master);
            _musicSlider.SetValueWithoutNotify(music);
            _effectsSlider.SetValueWithoutNotify(effects);
            _uiSlider.SetValueWithoutNotify(ui);
        }

        public void SetAudioMixerVolume(string volumeName, float dbVolume) => 
            _audioMixer.SetFloat(volumeName, dbVolume);
        
        private void SubscribeSliderEvents()
        {
            _masterSlider.onValueChanged.AddListener(InvokeOnMasterVolumeChanged);
            _musicSlider.onValueChanged.AddListener(InvokeOnMusicVolumeChanged);
            _effectsSlider.onValueChanged.AddListener(InvokeOnEffectsVolumeChanged);
            _uiSlider.onValueChanged.AddListener(InvokeOnUIVolumeChanged);
        }
        
        private void UnSubscribeSliderEvents()
        {
            _masterSlider.onValueChanged.RemoveListener(InvokeOnMasterVolumeChanged);
            _musicSlider.onValueChanged.RemoveListener(InvokeOnMusicVolumeChanged);
            _effectsSlider.onValueChanged.RemoveListener(InvokeOnEffectsVolumeChanged);
            _uiSlider.onValueChanged.RemoveListener(InvokeOnUIVolumeChanged);
        }

        private void InvokeOnApplyButtonClicked() => 
            OnApplyButtonClicked?.Invoke();
        
        private void InvokeOnCloseButtonClicked() => 
            OnCloseButtonClicked?.Invoke();
        
        private void InvokeOnMasterVolumeChanged(float value) => 
            OnMasterVolumeChanged?.Invoke(value);
        private void InvokeOnMusicVolumeChanged(float value) => 
            OnMusicVolumeChanged?.Invoke(value);
        private void InvokeOnEffectsVolumeChanged(float value) => 
            OnEffectsVolumeChanged?.Invoke(value);
        private void InvokeOnUIVolumeChanged(float value) => 
            OnUIVolumeChanged?.Invoke(value);
    }
}
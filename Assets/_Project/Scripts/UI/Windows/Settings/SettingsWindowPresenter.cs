using System;
using _Project.Scripts.Data.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Project.Scripts.UI.Windows.Settings
{
    public class SettingsWindowPresenter
    {
        private const string MasterVolumeName = "MasterVolume";
        private const string MusicVolumeName = "MusicVolume";
        private const string EffectsVolumeName = "EffectsVolume";
        private const string UIVolumeName = "UIVolume";
        
        private readonly SettingsWindowView _view;
        private readonly SettingsWindowModel _model;

        public SettingsWindowPresenter(SettingsWindowModel model, SettingsWindowView view)
        {
            _model = model;
            _view = view;
        }
        
        public void Initialize()
        {
            _view.Initialize();
            
            _view.OnWindowDestroy += CleanUp;
            _view.OnCloseButtonClicked += Close;
            _view.OnApplyButtonClicked += SaveSettingsAndClose;

            _view.OnMasterVolumeChanged += UpdateMasterVolume;
            _view.OnMusicVolumeChanged += UpdateMusicVolume;
            _view.OnEffectsVolumeChanged += UpdateEffectsVolume;
            _view.OnUIVolumeChanged += UpdateUIVolume;
            
            UpdateAudioMixerAndSlidersValues();
        }

        public async UniTask OpenAsync()
        {
            UpdateAudioMixerAndSlidersValues();
            await _view.OpenAsync();
        }

        private void CleanUp()
        {
            _view.OnWindowDestroy -= CleanUp;
            _view.OnCloseButtonClicked -= Close;
            _view.OnApplyButtonClicked -= SaveSettingsAndClose;

            _view.OnMasterVolumeChanged -= UpdateMasterVolume;
            _view.OnMusicVolumeChanged -= UpdateMusicVolume;
            _view.OnEffectsVolumeChanged -= UpdateEffectsVolume;
            _view.OnUIVolumeChanged -= UpdateUIVolume;
        }

        private async void SaveSettingsAndClose()
        {
            try
            {
                AudioSettingsData audioData = _model.AudioSettingsData;

                audioData.MasterVolume = _view.MasterVolume;
                audioData.MusicVolume = _view.MusicVolume;
                audioData.EffectsVolume = _view.EffectsVolume;
                audioData.UIVolume = _view.UIVolume;

                await _view.CloseAsync();
                await _model.SaveSettingsAsync();
            }
            catch (Exception e)
            {
               Debug.LogError(e);
            }
        }

        private async void Close()
        {
            try
            {
                await _view.CloseAsync();
                UpdateAudioMixerAndSlidersValues();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void UpdateAudioMixerAndSlidersValues()
        {
            AudioSettingsData data = _model.AudioSettingsData;
            _view.UpdateSlidersValues(data.MasterVolume, data.MusicVolume, data.EffectsVolume, data.UIVolume);

            UpdateMasterVolume(data.MasterVolume);
            UpdateMusicVolume(data.MusicVolume);
            UpdateEffectsVolume(data.EffectsVolume);
            UpdateUIVolume(data.UIVolume);
        }

        private void UpdateMasterVolume(float volume) =>
            UpdateAudioMixerVolume(MasterVolumeName, volume);

        private void UpdateMusicVolume(float volume) =>
            UpdateAudioMixerVolume(MusicVolumeName, volume);

        private void UpdateEffectsVolume(float volume) =>
            UpdateAudioMixerVolume(EffectsVolumeName, volume);

        private void UpdateUIVolume(float volume) =>
            UpdateAudioMixerVolume(UIVolumeName, volume);

        private void UpdateAudioMixerVolume(string volumeName, float volume)
        {
            float dbVolume = _model.ConvertVolumeToDecibel(volume);
            _view.UpdateAudioMixerVolume(volumeName, dbVolume);
        }
    }
}
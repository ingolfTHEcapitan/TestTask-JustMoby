using System;
using _Project.Scripts.Data.Player;
using UnityEngine;

namespace _Project.Scripts.UI.Windows.Settings
{
    public class SettingsPresenter: IDisposable
    {
        private SettingsView _view;
        private SettingsModel _model;
        private const string MasterVolumeName = "MasterVolume";
        private const string MusicVolumeName = "MusicVolume";
        private const string EffectsVolumeName = "EffectsVolume";
        private const string UIVolumeName = "UIVolume";

        public SettingsPresenter(SettingsModel model) => 
            _model = model;

        public void Construct(SettingsView view) => 
            _view = view;

        public void Initialize()
        {
            _view.OnOpen += SyncViewWithModel;
            _view.OnCloseButtonClicked += HandleClose;
            _view.OnApplyButtonClicked += HandleAppleSettings;

            _view.OnMasterVolumeChanged += UpdateMasterVolume;
            _view.OnMusicVolumeChanged += UpdateMusicVolume;
            _view.OnEffectsVolumeChanged += UpdateEffectsVolume;
            _view.OnUIVolumeChanged += UpdateUIVolume;
        }
        
        public void Dispose()
        {
            _view.OnOpen -= SyncViewWithModel;
            _view.OnCloseButtonClicked -= HandleClose;
            _view.OnApplyButtonClicked -= HandleAppleSettings;

            _view.OnMasterVolumeChanged -= UpdateMasterVolume;
            _view.OnMusicVolumeChanged -= UpdateMusicVolume;
            _view.OnEffectsVolumeChanged -= UpdateEffectsVolume;
            _view.OnUIVolumeChanged -= UpdateUIVolume;
        }


        private void SyncViewWithModel()
        {
            AudioSettingsData audioData = _model.AudioSettingsData;
            _view.SetSlidersValues(audioData.MasterVolume, audioData.MusicVolume, audioData.EffectsVolume, audioData.UIVolume);
            
            UpdateMasterVolume(audioData.MasterVolume);
            UpdateMusicVolume(audioData.MusicVolume);
            UpdateEffectsVolume(audioData.EffectsVolume);
            UpdateUIVolume(audioData.UIVolume);
        }

        private async void HandleAppleSettings()
        {
            AudioSettingsData audioData = _model.AudioSettingsData;
            
            audioData.MasterVolume = _view.MasterVolume;
            audioData.MusicVolume = _view.MusicVolume;
            audioData.EffectsVolume = _view.EffectsVolume;
            audioData.UIVolume = _view.UIVolume;
            
            await _view.CloseAsync();
            await _model.SaveSettingsAsync();
        }

        private async void HandleClose()
        {
            await _view.CloseAsync();
            SyncViewWithModel();
        }


        private void UpdateAudioMixerVolume(string volumeName, float volume)
        {
            float dbVolume;
            if (volume < 1e-06)
                dbVolume = -80;
            else
                dbVolume = Mathf.Log10(volume) * 20;
            
            _view.SetAudioMixerVolume(volumeName, dbVolume);
        }
        
        
        private void UpdateMasterVolume(float volume) => 
            UpdateAudioMixerVolume(MasterVolumeName, volume);

        private void UpdateMusicVolume(float volume) => 
            UpdateAudioMixerVolume(MusicVolumeName, volume);

        private void UpdateEffectsVolume(float volume) => 
            UpdateAudioMixerVolume(EffectsVolumeName, volume);
        
        private void UpdateUIVolume(float volume) => 
            UpdateAudioMixerVolume(EffectsVolumeName, volume);
    }
}
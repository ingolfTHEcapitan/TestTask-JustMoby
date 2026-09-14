using System;
using _Project.Scripts.Data.Player;
using UnityEngine;

namespace _Project.Scripts.UI.Windows.Settings
{
    public class SettingsWindowPresenter: IDisposable
    {
        private SettingsWindowView _view;
        private SettingsWindowModel _model;
        private const string MasterVolumeName = "MasterVolume";
        private const string MusicVolumeName = "MusicVolume";
        private const string EffectsVolumeName = "EffectsVolume";
        private const string UIVolumeName = "UIVolume";

        public SettingsWindowPresenter(SettingsWindowModel model) =>
            _model = model;

        public void Construct(SettingsWindowView view) =>
            _view = view;

        public void Initialize()
        {
            _view.OnOpen += SyncViewWithModel;
            _view.OnCloseButtonClicked += Close;
            _view.OnApplyButtonClicked += SaveSettingsAndClose;

            _view.OnMasterVolumeChanged += UpdateMasterVolume;
            _view.OnMusicVolumeChanged += UpdateMusicVolume;
            _view.OnEffectsVolumeChanged += UpdateEffectsVolume;
            _view.OnUIVolumeChanged += UpdateUIVolume;
            
            SyncViewWithModel();
        }

        public void Dispose()
        {
            _view.OnOpen -= SyncViewWithModel;
            _view.OnCloseButtonClicked -= Close;
            _view.OnApplyButtonClicked -= SaveSettingsAndClose;

            _view.OnMasterVolumeChanged -= UpdateMasterVolume;
            _view.OnMusicVolumeChanged -= UpdateMusicVolume;
            _view.OnEffectsVolumeChanged -= UpdateEffectsVolume;
            _view.OnUIVolumeChanged -= UpdateUIVolume;
        }


        private async void SaveSettingsAndClose()
        {
            AudioSettingsData audioData = _model.AudioSettingsData;

            audioData.MasterVolume = _view.MasterVolume;
            audioData.MusicVolume = _view.MusicVolume;
            audioData.EffectsVolume = _view.EffectsVolume;
            audioData.UIVolume = _view.UIVolume;

            await _view.CloseAsync();
            await _model.SaveSettingsAsync();
        }

        private async void Close()
        {
            await _view.CloseAsync();
            SyncViewWithModel();
        }

        private void SyncViewWithModel()
        {
            AudioSettingsData data = _model.AudioSettingsData;
            _view.SetSlidersValues(data.MasterVolume, data.MusicVolume, data.EffectsVolume, data.UIVolume);

            UpdateMasterVolume(data.MasterVolume);
            UpdateMusicVolume(data.MusicVolume);
            UpdateEffectsVolume(data.EffectsVolume);
            UpdateUIVolume(data.UIVolume);
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
            UpdateAudioMixerVolume(UIVolumeName, volume);

    }
}
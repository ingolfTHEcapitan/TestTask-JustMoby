using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Windows.Settings
{
    public class SettingsWindowModel
    {
        private readonly IProgressService _progressService;
        private readonly ISaveLoadService _saveService;
        
        public AudioSettingsData AudioSettingsData => _progressService.PlayerProgress.AudioSettingsData;
        
        private SettingsWindowModel(IProgressService progressService, 
            [Inject(Id = SaveType.Coordinator)] ISaveLoadService saveService)
        {
            _saveService = saveService;
            _progressService = progressService;
        }

        public async UniTask SaveSettingsAsync() => 
            await _saveService.SaveProgressAsync(_progressService);
        
        public float ConvertVolumeToDecibel(float volume)
        {
            float dbVolume;
            if (volume < 1e-06)
                dbVolume = -80;
            else
                dbVolume = Mathf.Log10(volume) * 20;
            return dbVolume;
        }
    }
}
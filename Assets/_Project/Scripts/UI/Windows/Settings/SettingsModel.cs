using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.UI.Windows.Settings
{
    public class SettingsModel
    {
        private readonly IProgressService _progressService;
        private readonly ISaveLoadService _saveService;
        
        public AudioSettingsData AudioSettingsData => _progressService.PlayerProgress.AudioSettingsData;
        
        private SettingsModel(IProgressService progressService, 
            [Inject(Id = SaveType.Coordinator)] ISaveLoadService saveService)
        {
            _saveService = saveService;
            _progressService = progressService;
        }

        public async UniTask SaveSettingsAsync()
        {
            await _saveService.SaveProgressAsync(_progressService);
        }
    }
}
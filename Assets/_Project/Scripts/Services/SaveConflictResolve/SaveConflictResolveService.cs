using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.SaveConflictResolve.UI;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.SaveConflictResolve
{
    public class SaveConflictResolveService : ISaveConflictResolveService
    {
        private readonly IUIFactory _uiFactory;
        private readonly ISaveLoadCoordinator _saveLoadCoordinator;
        
        private SaveConflictResolveWindow _window;

        public SaveConflictResolveService(IUIFactory uiFactory, ISaveLoadCoordinator saveLoadCoordinator)
        {
            _saveLoadCoordinator = saveLoadCoordinator;
            _uiFactory = uiFactory;
        }

        public async UniTask CreateWindow()
        {
            _window = await _uiFactory.CreateSaveConflictResolveWindow();
            _saveLoadCoordinator.OnSaveConflictHappened += ResolveConflict;
        }
    
        private async UniTask<SaveType> ResolveConflict(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            _window.Construct(localProgress, cloudProgress);
            
            SaveType choice = await _window.Show();
            _window.Hide();
            return choice;
        }
    }
}
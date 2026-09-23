using System;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.SaveLoad;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.SaveConflictResolve.MVP
{
    public class SaveConflictResolveWindowModel
    {
        public event Func<PlayerProgress, PlayerProgress, UniTask<SaveType>> OnSaveConflictHappened;
        
        // M+
        private readonly PlayerProgress _localProgress;
        private readonly PlayerProgress _cloudProgress;
        private readonly ISaveLoadCoordinator _saveLoadCoordinator;
        
        public SaveConflictResolveWindowModel(PlayerProgress localProgress, PlayerProgress cloudProgress, ISaveLoadCoordinator saveLoadCoordinator)
        {
            _localProgress = localProgress;
            _cloudProgress = cloudProgress;
            _saveLoadCoordinator = saveLoadCoordinator;
        }
        
        //M
        public void Initialize() => 
            _saveLoadCoordinator.OnSaveConflictHappened += InvokeOnSaveConflictHappened;

        private async UniTask<SaveType> InvokeOnSaveConflictHappened(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            return await OnSaveConflictHappened.Invoke(localProgress, cloudProgress);
        }

        public bool IsLocalSaveIsNewer()=>
            _localProgress.LastSaveTimeUnix > _cloudProgress.LastSaveTimeUnix;
        
        public bool IsCloudSaveIsNewer()=>
            _cloudProgress.LastSaveTimeUnix > _localProgress.LastSaveTimeUnix;
        
        public bool IsLocalSaveEqualCloudSaveTime()=>
            _localProgress.LastSaveTimeUnix == _cloudProgress.LastSaveTimeUnix;
        
        
        
        
    }
}
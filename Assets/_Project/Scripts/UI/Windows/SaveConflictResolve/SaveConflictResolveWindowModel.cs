using System;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.SaveLoad;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.SaveConflictResolve
{
    public class SaveConflictResolveWindowModel: IDisposable
    {
        public event Func<PlayerProgress, PlayerProgress, UniTask<SaveType>> OnSaveConflictHappened;

        private readonly ISaveLoadCoordinator _saveLoadCoordinator;
        private PlayerProgress _localProgress;
        private PlayerProgress _cloudProgress;

        public SaveConflictResolveWindowModel(ISaveLoadCoordinator saveLoadCoordinator) => 
            _saveLoadCoordinator = saveLoadCoordinator;

        public void Initialize() => 
            _saveLoadCoordinator.OnSaveConflictHappened += InvokeOnSaveConflictHappened;

        public void Dispose() => 
            _saveLoadCoordinator.OnSaveConflictHappened -= InvokeOnSaveConflictHappened;

        public bool IsLocalSaveNewer()=>
            _localProgress.LastSaveTimeUnix > _cloudProgress.LastSaveTimeUnix;

        public bool IsCloudSaveNewer()=>
            _cloudProgress.LastSaveTimeUnix > _localProgress.LastSaveTimeUnix;

        public bool IsLocalSaveEqualCloudSaveTime()=>
            _localProgress.LastSaveTimeUnix == _cloudProgress.LastSaveTimeUnix;

        private async UniTask<SaveType> InvokeOnSaveConflictHappened(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            _localProgress = localProgress;
            _cloudProgress = cloudProgress;
            return await OnSaveConflictHappened.Invoke(localProgress, cloudProgress);
        }
    }
}
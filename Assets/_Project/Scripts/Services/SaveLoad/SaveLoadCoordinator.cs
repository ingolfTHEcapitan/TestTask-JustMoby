using System;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.Authentication;
using _Project.Scripts.Services.NetworkAccessibility;
using _Project.Scripts.Services.Progress;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.SaveLoad
{
    public class SaveLoadCoordinator: ISaveLoadCoordinator
    {
        public event Func<PlayerProgress, PlayerProgress, UniTask<SaveType>> OnSaveConflictHappened;
        
        private readonly INetworkAccessibilityService _networkAccessibility;
        private readonly ISaveLoadService _localSaveService;
        private readonly ISaveLoadService _cloudSaveService;
        private readonly IAuthService _authService;
        private readonly IProgressService _progressService;

        public SaveLoadCoordinator(INetworkAccessibilityService networkAccessibility, IAuthService authService, IProgressService progressService,
            [Inject(Id = SaveType.Local)] ISaveLoadService localSaveService, [Inject(Id = SaveType.Cloud)] ISaveLoadService cloudSaveService)
        {
            _networkAccessibility = networkAccessibility;
            _authService = authService;
            _cloudSaveService = cloudSaveService;
            _localSaveService = localSaveService;
            _progressService = progressService;
        }

        public async UniTask SaveProgressAsync(IProgressService progressService)
        {
            progressService.PlayerProgress.LastSaveTimeUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            await _localSaveService.SaveProgressAsync(progressService);
            
            if (await HasInternetAsync() && _authService.IsSignedIn)
            {
                try
                {
                    await _cloudSaveService.SaveProgressAsync(progressService);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[SAVE COORDINATOR] Failed save to cloud, message: {e.Message}");
                }
            }
                
        }

        public async UniTask<PlayerProgress> LoadProgressAsync()
        {
            PlayerProgress localProgress = await _localSaveService.LoadProgressAsync();
            
            if (!await HasInternetAsync() || !_authService.IsSignedIn)
            {
                Debug.LogWarning("[SAVE COORDINATOR] No access to internet or no authorization. Local save has been loaded");
                return localProgress;
            }

            try
            {
                PlayerProgress cloudProgress = await _cloudSaveService.LoadProgressAsync();
                
                bool cloudSaveIsNewer = cloudProgress.LastSaveTimeUnix > localProgress.LastSaveTimeUnix;
                bool localSaveIsNewer = localProgress.LastSaveTimeUnix > cloudProgress.LastSaveTimeUnix;

                if (cloudSaveIsNewer)
                    return await LoadCloudSaveAsync(cloudProgress);
                
                if (localSaveIsNewer)
                    return await ResolveSaveConflictAsync(localProgress, cloudProgress);
                
                return localProgress;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SAVE COORDINATOR] Synchronization cloud error. Local save has been loaded. Message: {e.Message}");
                return localProgress;
            }
        }

        private async UniTask<PlayerProgress> ResolveSaveConflictAsync(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            if (OnSaveConflictHappened == null)
                return await LoadLocalSaveAsync(localProgress);
            
            SaveType choice = await OnSaveConflictHappened.Invoke(localProgress, cloudProgress);
            
            if (choice == SaveType.Local)
                return await LoadLocalSaveAsync(localProgress);
            
            return await LoadCloudSaveAsync(cloudProgress);
        }

        private async UniTask<PlayerProgress> LoadCloudSaveAsync(PlayerProgress cloudProgress)
        {
            _progressService.PlayerProgress = cloudProgress;
            await _localSaveService.SaveProgressAsync(_progressService);
            return cloudProgress;
        }

        private async UniTask<PlayerProgress> LoadLocalSaveAsync(PlayerProgress localProgress)
        {
            _progressService.PlayerProgress = localProgress;
            await _cloudSaveService.SaveProgressAsync(_progressService);
            return localProgress;
        }

        private async UniTask<bool> HasInternetAsync() => 
            await _networkAccessibility.CheckNetworkConnectionAsync();
    }
}
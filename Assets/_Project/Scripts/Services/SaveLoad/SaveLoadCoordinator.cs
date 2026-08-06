using System;
using System.Threading.Tasks;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.Authentication;
using _Project.Scripts.Services.NetworkAccessibility;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.SaveLoad
{
    public class SaveLoadCoordinator: ISaveLoadCoordinator
    {
        public event Func<PlayerProgress, PlayerProgress, UniTask<SaveType>> OnSaveConflictHappened;
        
        private readonly NetworkAccessibilityService _networkAccessibility;
        private readonly ISaveLoadService _localSaveService;
        private readonly ISaveLoadService _cloudSaveService;
        private readonly IAuthService _authService;
        private readonly IProgressService _progressService;

        public SaveLoadCoordinator(NetworkAccessibilityService networkAccessibility, IAuthService authService, IProgressService progressService,
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
                catch (Exception exception)
                {
                    Debug.LogWarning($"[{GetType().Name}] Не удалось сохранить в облако: {exception.Message}");
                }
            }
                
        }

        public async UniTask<PlayerProgress> LoadProgressAsync()
        {
            PlayerProgress localProgress = await _localSaveService.LoadProgressAsync();
            
            if (!await HasInternetAsync() || !_authService.IsSignedIn)
            {
                Debug.LogWarning($"[{GetType().Name}] Оффлайн режим или нет авторизации. Загружено локальное сохранение");
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
                Debug.LogError($"[{GetType().Name}] Ошибка при синхронизации с облаком: {e.Message}. Загружено локальное сохранение");
                return localProgress;
            }
        }

        private async Task<PlayerProgress> ResolveSaveConflictAsync(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            Debug.LogWarning($"[{GetType().Name}] Обнаружен конфликт: Локальное сохранение новее облачного");
            
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
            Debug.Log($"[{GetType().Name}] Загружено облачное сохранение, локальное было обновлено");
            return cloudProgress;
        }

        private async UniTask<PlayerProgress> LoadLocalSaveAsync(PlayerProgress localProgress)
        {
            _progressService.PlayerProgress = localProgress;
            await _cloudSaveService.SaveProgressAsync(_progressService);
            Debug.Log($"[{GetType().Name}] Загружено локально сохранение, Облачное было обновлено");
            return localProgress;
        }

        private async Task<bool> HasInternetAsync() => 
            await _networkAccessibility.CheckNetworkConnectionAsync();
    }
}
using System;
using System.Threading.Tasks;
using _Project.Scripts.Data;
using _Project.Scripts.Services.Authentication;
using _Project.Scripts.Services.Factory.UIFactory;
using _Project.Scripts.Services.NetworkAccessibility;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.UI.Windows.SaveConflictResolve;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.SaveLoad
{
    public class SaveLoadCoordinator: ISaveLoadService
    {
        private readonly NetworkAccessibilityService _networkAccessibility;
        private readonly ISaveLoadService _localSaveService;
        private readonly ISaveLoadService _cloudSaveService;
        private readonly IUIFactory _uiFactory;
        private readonly IAuthService _authService;
        private readonly IProgressService _progressService;

        public SaveLoadCoordinator(NetworkAccessibilityService networkAccessibility, IUIFactory uiFactory, IAuthService authService,
            [Inject(Id = SaveType.Local)] ISaveLoadService localSaveService, [Inject(Id = SaveType.Cloud)] ISaveLoadService cloudSaveService,
            IProgressService progressService)
        {
            _networkAccessibility = networkAccessibility;
            _authService = authService;
            _uiFactory = uiFactory;
            _cloudSaveService = cloudSaveService;
            _localSaveService = localSaveService;
            _progressService = progressService;
        }

        public async UniTask SaveProgressAsync(IProgressService progressService)
        {
            await _localSaveService.SaveProgressAsync(progressService);
            
            if (await HasInternet() && _authService.IsSignedIn)
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
            
            if (!await HasInternet() || !_authService.IsSignedIn)
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
                    return LoadCloudSave(cloudProgress);
                
                if (localSaveIsNewer)
                    return await ResolveSaveConflict(localProgress, cloudProgress);
                
                return localProgress;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{GetType().Name}] Ошибка при синхронизации с облаком: {e.Message}. Загружено локальное сохранение");
                return localProgress;
            }
        }

        private async Task<PlayerProgress> ResolveSaveConflict(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            Debug.LogWarning($"[{GetType().Name}] Обнаружен конфликт: Локальное сохранение новее облачного");
                    
            SaveConflictResolveWindow conflictResolveWindow = await _uiFactory.CreateSaveConflictResolveWindow(localProgress, cloudProgress);
            SaveType choice = await conflictResolveWindow.Show();
            conflictResolveWindow.Close();

            if (choice == SaveType.Local)
                return LoadLocalSave(localProgress);
            
            return LoadCloudSave(cloudProgress);

        }

        private PlayerProgress LoadCloudSave(PlayerProgress cloudProgress)
        {
            _progressService.PlayerProgress = cloudProgress;
            _localSaveService.SaveProgressAsync(_progressService);
            Debug.Log($"[{GetType().Name}] Загружено облачное сохранение, локальное было обновлено");
            return cloudProgress;
        }

        private PlayerProgress LoadLocalSave(PlayerProgress localProgress)
        {
            _progressService.PlayerProgress = localProgress;
            _cloudSaveService.SaveProgressAsync(_progressService);
            Debug.Log($"[{GetType().Name}] Загружено локально сохранение, Облачное было обновлено");
            return localProgress;
        }

        private async Task<bool> HasInternet() => 
            await _networkAccessibility.CheckNetworkConnectionAsync();
    }
}
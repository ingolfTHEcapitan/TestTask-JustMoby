using System;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.SaveConflictResolve.UI;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Services.SaveConflictResolve
{
    public class SaveConflictResolveService : ISaveConflictResolveService, IDisposable
    {
        private readonly IUIFactory _uiFactory;
        private readonly ISaveLoadCoordinator _saveLoadCoordinator;
        
        private readonly CursorController _cursorController;
        private readonly SaveTimeFormater _saveTimeFormater;
        private readonly Transform _uiParent;

        public SaveConflictResolveService(IUIFactory uiFactory, ISaveLoadCoordinator saveLoadCoordinator, 
            CursorController cursorController, SaveTimeFormater saveTimeFormater, Transform uiParent)
        {
            _cursorController = cursorController;
            _saveLoadCoordinator = saveLoadCoordinator;
            _uiFactory = uiFactory;
            _saveTimeFormater = saveTimeFormater;
            _uiParent = uiParent;
        }

        public void Initialize() => 
            _saveLoadCoordinator.OnSaveConflictHappened += ResolveConflict;
        
        public void Dispose() => 
            _saveLoadCoordinator.OnSaveConflictHappened -= ResolveConflict;

        private async UniTask<SaveType> ResolveConflict(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            SaveConflictResolveWindow window = await _uiFactory.CreateSaveConflictResolveWindowAsync(_uiParent);
            
            window.Construct(localProgress, cloudProgress, _cursorController, _saveTimeFormater);
            
            SaveType choice = await window.ShowAsync();
            await window.CloseAsync();

            if (window) 
                Object.Destroy(window.gameObject);
            
            return choice;
        }
    }
}
using System;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.UI.Windows.SaveConflictResolve
{
    public class SaveConflictResolveService : ISaveConflictResolveService, IDisposable
    {
        //P
        private readonly IUIFactory _uiFactory;
        //M+
        private readonly ISaveLoadCoordinator _saveLoadCoordinator;
        
        //P
        private readonly CursorController _cursorController;
        private readonly SaveTimeFormatter _saveTimeFormatter;
        private readonly Transform _uiParent;

        public SaveConflictResolveService(IUIFactory uiFactory, ISaveLoadCoordinator saveLoadCoordinator, 
            CursorController cursorController, SaveTimeFormatter saveTimeFormatter, Transform uiParent)
        {
            //P
            _cursorController = cursorController;
            //M+
            _saveLoadCoordinator = saveLoadCoordinator;
            //P
            _uiFactory = uiFactory;
            _saveTimeFormatter = saveTimeFormatter;
            _uiParent = uiParent;
        }

        //M+
        public void Initialize() => 
            _saveLoadCoordinator.OnSaveConflictHappened += ResolveConflict;
        //M
        public void Dispose() => 
            _saveLoadCoordinator.OnSaveConflictHappened -= ResolveConflict;

        //P
        private async UniTask<SaveType> ResolveConflict(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            //P
            SaveConflictResolveWindow window = await _uiFactory.CreateSaveConflictResolveWindowAsync(_uiParent);
            window.Construct(localProgress, cloudProgress, _cursorController, _saveTimeFormatter);
            
            SaveType choice = await window.ShowAsync();
            await window.CloseAsync();

            if (window) 
                Object.Destroy(window.gameObject);
            
            return choice;
        }
    }
}
using System;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.SaveConflictResolve
{
    public class SaveConflictResolveWindowPresenter: IDisposable
    {
        private readonly SaveConflictResolveWindowModel _model;
        private readonly SaveConflictResolveWindowView _view;
        private readonly CursorController _cursorController;
        private readonly SaveTimeFormatter _saveTimeFormatter;
        
        private UniTaskCompletionSource<SaveType> _taskCompletionSource;
        
        public SaveConflictResolveWindowPresenter(SaveConflictResolveWindowModel model, SaveConflictResolveWindowView view, CursorController cursorController, SaveTimeFormatter saveTimeFormatter)
        {
            _model = model;
            _view = view;
            _cursorController = cursorController;
            _saveTimeFormatter = saveTimeFormatter;
        }

        public void Initialize()
        {
            _model.Initialize();
            _view.Initialize();

            _model.OnSaveConflictHappened += ResolveConflict;
            _view.OnLocalSaveButtonClicked += ChoiceLocalSave;
            _view.OnCloudSaveButtonClicked += ChoiceCloudSave;
        }

        private async UniTask<SaveType> ResolveConflict(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            _view.Open();
            _cursorController.SetCursorVisible(true);
            
            string local = _saveTimeFormatter.Format(localProgress.LastSaveTimeUnix);
            string cloud = _saveTimeFormatter.Format(cloudProgress.LastSaveTimeUnix);
            
            _view.UpdateSaveDateText(local, cloud);
            ChoiceSaveDateTextColor();
            
            _taskCompletionSource = new UniTaskCompletionSource<SaveType>();
            SaveType result = await _taskCompletionSource.Task;
            _taskCompletionSource = null;
            
            await _view.CloseAsync();
            _cursorController.SetCursorVisible(false);
            
            return result;
        }

        public void Dispose()
        {
            _taskCompletionSource = null;
            _model.Dispose();
            _model.OnSaveConflictHappened -= ResolveConflict;
            _view.OnLocalSaveButtonClicked -= ChoiceLocalSave;
            _view.OnCloudSaveButtonClicked -= ChoiceCloudSave;
        }

        private void ChoiceSaveDateTextColor()
        {
            if (_model.IsLocalSaveNewer())
            {
                _view.SetLocalDateTextColorNew();
            }
            else if (_model.IsCloudSaveNewer())
            {
               _view.SetCloudDateTextColorNew();
            }
            else if (_model.IsLocalSaveEqualCloudSaveTime())
            {
                _view.SetSaveDateTextColorDefault();
            }
        }

        private void ChoiceCloudSave() => 
            _taskCompletionSource?.TrySetResult(SaveType.Cloud);

        private void ChoiceLocalSave() => 
            _taskCompletionSource?.TrySetResult(SaveType.Local);
    }
}
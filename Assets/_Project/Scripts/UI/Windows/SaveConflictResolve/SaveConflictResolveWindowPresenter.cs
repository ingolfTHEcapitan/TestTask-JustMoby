using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.SaveConflictResolve
{
    public class SaveConflictResolveWindowPresenter
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

            _view.OnWindowDestroy += CleanUp;
            _view.OnLocalSaveButtonClicked += ChoiceLocalSave;
            _view.OnCloudSaveButtonClicked += ChoiceCloudSave;
            _model.OnSaveConflictHappened += ResolveConflictAsync;
        }

        private void CleanUp()
        {
            _view.OnWindowDestroy -= CleanUp;
            _view.OnLocalSaveButtonClicked -= ChoiceLocalSave;
            _view.OnCloudSaveButtonClicked -= ChoiceCloudSave;
            _model.OnSaveConflictHappened -= ResolveConflictAsync;
            _taskCompletionSource = null;
            _model.Dispose();
        }

        private async UniTask<SaveType> ResolveConflictAsync(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            string localDate = _saveTimeFormatter.Format(localProgress.LastSaveTimeUnix);
            string cloudDate = _saveTimeFormatter.Format(cloudProgress.LastSaveTimeUnix);

            SaveNewestState saveState = GetSaveNewestState();
            _view.UpdateSaveInfo(localDate, cloudDate, saveState);
            
            await _view.OpenAsync();
            _cursorController.SetCursorVisible(true);

            _taskCompletionSource = new UniTaskCompletionSource<SaveType>();
            SaveType result = await _taskCompletionSource.Task;
            _taskCompletionSource = null;

            _cursorController.SetCursorVisible(false);
            await _view.CloseAsync();

            return result;
        }

        private SaveNewestState GetSaveNewestState()
        {
            if (_model.LocalSaveIsNewer)
                return SaveNewestState.LocalNewer;
            if (_model.CloudSaveIsNewer)
                return SaveNewestState.CloudNewer;

            return SaveNewestState.Equals;
        }

        private void ChoiceCloudSave() => 
            _taskCompletionSource?.TrySetResult(SaveType.Cloud);

        private void ChoiceLocalSave() => 
            _taskCompletionSource?.TrySetResult(SaveType.Local);
    }
}
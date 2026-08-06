using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Services.SaveConflictResolve.UI
{
    public class SaveConflictResolveWindow: MonoBehaviour
    {
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private Button _localSaveButton;
        [SerializeField] private Button _cloudSaveButton;
        [SerializeField] private TextMeshProUGUI _localDateText;
        [SerializeField] private TextMeshProUGUI _cloudDateText;
        [SerializeField] private Color _newSaveColor = Color.green;
        [SerializeField] private Color _oldSaveColor = Color.red;
        [SerializeField] private Color _defaultSaveColor = Color.black;

        private UniTaskCompletionSource<SaveType> _taskCompletionSource;
        private PlayerProgress _localProgress;
        private PlayerProgress _cloudProgress;
        private CursorController _cursorController;
        private SaveTimeFormater _saveTimeFormater;

        public void Construct(PlayerProgress localProgress, PlayerProgress cloudProgress,
            CursorController cursorController, SaveTimeFormater saveTimeFormater)
        {
            _cursorController = cursorController;
            _localProgress = localProgress;
            _cloudProgress = cloudProgress;
            _saveTimeFormater = saveTimeFormater;
        }
        
        public void Awake()
        {
            _localSaveButton.onClick.AddListener(ChoiceLocalSave);
            _cloudSaveButton.onClick.AddListener(ChoiceCloudSave);
            gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            _localSaveButton.onClick.RemoveListener(ChoiceLocalSave);
            _cloudSaveButton.onClick.RemoveListener(ChoiceCloudSave);
            Destroy(gameObject);
        }

        public async UniTask<SaveType> ShowAsync()
        {
            gameObject.SetActive(true); 
            _cursorController.SetCursorVisible(true);
            _windowAnimation.AnimateOpen();
            
            _localDateText.text = $"Device save date\n{_saveTimeFormater.GetFormatedSaveTime(_localProgress.LastSaveTimeUnix)}";
            _cloudDateText.text = $"Cloud save date\n{_saveTimeFormater.GetFormatedSaveTime(_cloudProgress.LastSaveTimeUnix)}";
            
            ChoiceSaveDateTextColor();
            
            _taskCompletionSource = new UniTaskCompletionSource<SaveType>();
            SaveType result = await _taskCompletionSource.Task;
            
            _taskCompletionSource = null; 
            return result;
        }

        public async UniTask CloseAsync()
        {
            _taskCompletionSource = null;
            await _windowAnimation.AnimateCloseAsync();
            Destroy(gameObject);
        }

        private void ChoiceSaveDateTextColor()
        {
            if (_localProgress.LastSaveTimeUnix > _cloudProgress.LastSaveTimeUnix)
            {
                _localDateText.color = _newSaveColor;
                _cloudDateText.color = _oldSaveColor;
            }
            else if (_cloudProgress.LastSaveTimeUnix > _localProgress.LastSaveTimeUnix)
            {
                _localDateText.color = _oldSaveColor;
                _cloudDateText.color = _newSaveColor;
            }
            else if (_localProgress.LastSaveTimeUnix == _cloudProgress.LastSaveTimeUnix)
            {
                _localDateText.color = _defaultSaveColor;
                _cloudDateText.color = _defaultSaveColor;
            }
        }

        private void ChoiceCloudSave() => 
            _taskCompletionSource?.TrySetResult(SaveType.Cloud);

        private void ChoiceLocalSave() => 
            _taskCompletionSource?.TrySetResult(SaveType.Local);
    }
}
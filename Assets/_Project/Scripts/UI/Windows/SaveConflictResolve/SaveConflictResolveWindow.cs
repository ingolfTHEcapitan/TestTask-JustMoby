using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.SaveConflictResolve
{
    public class SaveConflictResolveWindow: MonoBehaviour
    {
        // V
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private Button _localSaveButton;
        [SerializeField] private Button _cloudSaveButton;
        [SerializeField] private TextMeshProUGUI _localDateText;
        [SerializeField] private TextMeshProUGUI _cloudDateText;
        [SerializeField] private Color _newSaveColor = Color.green;
        [SerializeField] private Color _oldSaveColor = Color.red;
        [SerializeField] private Color _defaultSaveColor = Color.black;
        
        //P
        private UniTaskCompletionSource<SaveType> _taskCompletionSource;
        // M+
        private PlayerProgress _localProgress;
        private PlayerProgress _cloudProgress;
        
        // P
        private CursorController _cursorController;
        private SaveTimeFormatter _saveTimeFormatter;

        public void Construct(PlayerProgress localProgress, PlayerProgress cloudProgress,
            CursorController cursorController, SaveTimeFormatter saveTimeFormatter)
        {
            //P
            _cursorController = cursorController;
            // M+
            _localProgress = localProgress;
            _cloudProgress = cloudProgress;
            // P
            _saveTimeFormatter = saveTimeFormatter;
        }
        
        // V
        public void Awake()
        {
            // V
            _localSaveButton.onClick.AddListener(ChoiceLocalSave);
            _cloudSaveButton.onClick.AddListener(ChoiceCloudSave);
            gameObject.SetActive(false);
        }
        
        // V
        private void OnDestroy()
        {
            _localSaveButton.onClick.RemoveListener(ChoiceLocalSave);
            _cloudSaveButton.onClick.RemoveListener(ChoiceCloudSave);
        }

        // MVP
        public async UniTask<SaveType> ShowAsync()
        {
            // V
            gameObject.SetActive(true); 
            // P
            _cursorController.SetCursorVisible(true);
            // V
            _windowAnimation.AnimateOpen();
            
            // V // Формат текста - // P
            _localDateText.text = $"Device save date\n{_saveTimeFormatter.Format(_localProgress.LastSaveTimeUnix)}";
            _cloudDateText.text = $"Cloud save date\n{_saveTimeFormatter.Format(_cloudProgress.LastSaveTimeUnix)}";
            
            //P
            ChoiceSaveDateTextColor();
            
            //P
            _taskCompletionSource = new UniTaskCompletionSource<SaveType>();
            SaveType result = await _taskCompletionSource.Task;
            
            _taskCompletionSource = null; 
            return result;
        }

        public async UniTask CloseAsync()
        {   
            //P
            _taskCompletionSource = null;
            //V
            await _windowAnimation.AnimateCloseAsync();
            Destroy(gameObject);
        }

        //P
        private void ChoiceSaveDateTextColor()
        {
            // M+
            if (_localProgress.LastSaveTimeUnix > _cloudProgress.LastSaveTimeUnix)
            {
                // V
                _localDateText.color = _newSaveColor;
                _cloudDateText.color = _oldSaveColor;
            }
            // M+
            else if (_cloudProgress.LastSaveTimeUnix > _localProgress.LastSaveTimeUnix)
            {
                // V
                _localDateText.color = _oldSaveColor;
                _cloudDateText.color = _newSaveColor;
            }
            // M+
            else if (_localProgress.LastSaveTimeUnix == _cloudProgress.LastSaveTimeUnix)
            {
                // V
                _localDateText.color = _defaultSaveColor;
                _cloudDateText.color = _defaultSaveColor;
            }
        }

        // P
        private void ChoiceCloudSave() => 
            _taskCompletionSource?.TrySetResult(SaveType.Cloud);

        // P
        private void ChoiceLocalSave() => 
            _taskCompletionSource?.TrySetResult(SaveType.Local);
    }
}
using _Project.Scripts.Data.Player;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.SaveLoad;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Services.SaveConflictResolve.UI
{
    public class SaveConflictResolveWindow: MonoBehaviour
    {
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
        
        public void Construct(PlayerProgress localProgress, PlayerProgress cloudProgress)
        {
            _localProgress = localProgress;
            _cloudProgress = cloudProgress;
        }
        
        public void Awake()
        {
            _localSaveButton.onClick.AddListener(ChoiceLocalSave);
            _cloudSaveButton.onClick.AddListener(ChoiceCloudSave);
            gameObject.SetActive(false);
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnDestroy()
        {
            _localSaveButton.onClick.RemoveListener(ChoiceLocalSave);
            _cloudSaveButton.onClick.RemoveListener(ChoiceCloudSave);
        }

        public async UniTask<SaveType> Show()
        {
            gameObject.SetActive(true);
            CursorController.SetCursorVisible(true);
            
            _localDateText.text = $"Device save date\n{_localProgress.GetFormatedSaveTime()}";
            _cloudDateText.text = $"Cloud save date\n{_cloudProgress.GetFormatedSaveTime()}";
            
            ChoiceSaveDateTextColor();
            
            _taskCompletionSource = new UniTaskCompletionSource<SaveType>();
            SaveType result = await _taskCompletionSource.Task;
            
            _taskCompletionSource = null; 
            return result;
        }

        public void Hide()
        {
            CursorController.SetCursorVisible(false);
            _taskCompletionSource = null;
            gameObject.SetActive(false);
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
            _taskCompletionSource.TrySetResult(SaveType.Cloud);

        private void ChoiceLocalSave() => 
            _taskCompletionSource.TrySetResult(SaveType.Local);
    }
}
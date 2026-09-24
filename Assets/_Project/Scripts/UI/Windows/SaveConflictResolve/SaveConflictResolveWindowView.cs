using System;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.SaveConflictResolve
{
    public class SaveConflictResolveWindowView: MonoBehaviour, IWindow
    {
        public event Action OnLocalSaveButtonClicked;
        public event Action OnCloudSaveButtonClicked;
            
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _windowContent;
        [SerializeField] private Button _localSaveButton;
        [SerializeField] private Button _cloudSaveButton;
        [SerializeField] private TextMeshProUGUI _localDateText;
        [SerializeField] private TextMeshProUGUI _cloudDateText;
        [SerializeField] private Color _newSaveColor = Color.green;
        [SerializeField] private Color _oldSaveColor = Color.red;
        [SerializeField] private Color _defaultSaveColor = Color.black;

        private void Awake()
        {
            _windowContent.SetActive(false);
            DontDestroyOnLoad(gameObject);
        }

        public void Initialize()
        {
            _localSaveButton.onClick.AddListener(InvokeOnLocalSaveButtonClicked);
            _cloudSaveButton.onClick.AddListener(InvokeOnCloudSaveButtonClicked);
        }

        private void OnDestroy()
        {
            _localSaveButton.onClick.RemoveListener(InvokeOnLocalSaveButtonClicked);
            _cloudSaveButton.onClick.RemoveListener(InvokeOnCloudSaveButtonClicked);
        }

        public void Open()
        {
            _windowContent.SetActive(true); 
            _windowAnimation.AnimateOpen();
        }
        
        public async UniTask CloseAsync()
        {
            await _windowAnimation.AnimateCloseAsync();
            _windowContent.SetActive(false); 
        }

        public void UpdateSaveDateText(string localProgressText, string cloudProgressText)
        {
            _localDateText.text = $"Device save date\n{localProgressText}";
            _cloudDateText.text = $"Cloud save date\n{cloudProgressText}";
        }

        public void SetLocalDateTextColorNew()
        {
            _localDateText.color = _newSaveColor;
            _cloudDateText.color = _oldSaveColor;
        }
        
        public void SetCloudDateTextColorNew()
        {
            _localDateText.color = _oldSaveColor;
            _cloudDateText.color = _newSaveColor;
        }
        
        public void SetSaveDateTextColorDefault()
        {
            _localDateText.color = _defaultSaveColor;
            _cloudDateText.color = _defaultSaveColor;
        }
        
        private void InvokeOnLocalSaveButtonClicked() => 
            OnLocalSaveButtonClicked?.Invoke();

        private void InvokeOnCloudSaveButtonClicked() => 
            OnCloudSaveButtonClicked?.Invoke();
    }
}
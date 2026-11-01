using System;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.SaveConflictResolve
{
    public class SaveConflictResolveWindowView : MonoBehaviour, IWindow
    {
        public event Action OnWindowDestroy;
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
        }

        public void Initialize()
        {
            _localSaveButton.onClick.AddListener(InvokeOnLocalSaveButtonClicked);
            _cloudSaveButton.onClick.AddListener(InvokeOnCloudSaveButtonClicked);
        }

        private void OnDestroy()
        {
            OnWindowDestroy?.Invoke();
            _localSaveButton.onClick.RemoveListener(InvokeOnLocalSaveButtonClicked);
            _cloudSaveButton.onClick.RemoveListener(InvokeOnCloudSaveButtonClicked);
        }

        public async UniTask OpenAsync()
        {
            _windowContent.SetActive(true); 
            await _windowAnimation.AnimateOpenAsync();
        }
        
        public async UniTask CloseAsync()
        {
            await _windowAnimation.AnimateCloseAsync();
            _windowContent.SetActive(false); 
        }

        public void UpdateSaveInfo(string localDate, string cloudDate, SaveNewestState saveState)
        {
            UpdateSaveDateText(localDate, cloudDate);
            ChoiceSaveDateTextColor(saveState);
        }

        private void ChoiceSaveDateTextColor(SaveNewestState saveState)
        {
            switch (saveState)
            {
                case SaveNewestState.LocalNewer:
                    _localDateText.color = _newSaveColor;
                    _cloudDateText.color = _oldSaveColor;
                    break;
                case SaveNewestState.CloudNewer:
                    _cloudDateText.color = _newSaveColor;
                    _localDateText.color = _oldSaveColor;
                    break;
                case SaveNewestState.Equals:
                    _localDateText.color = _defaultSaveColor;
                    _cloudDateText.color = _defaultSaveColor;
                    break;
            }
        }
        
        private void UpdateSaveDateText(string localDate, string cloudDate)
        {
            _localDateText.text = $"Device save date\n{localDate}";
            _cloudDateText.text = $"Cloud save date\n{cloudDate}";
        }

        private void InvokeOnLocalSaveButtonClicked() => 
            OnLocalSaveButtonClicked?.Invoke();

        private void InvokeOnCloudSaveButtonClicked() => 
            OnCloudSaveButtonClicked?.Invoke();
    }
}
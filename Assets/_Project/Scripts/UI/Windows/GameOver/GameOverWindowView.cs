using System;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.GameOver
{
    public class GameOverWindowView : MonoBehaviour
    {
        public event Action OnReviveButtonClicked;
        public event Action OnLoadSaveButtonClicked;
        
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _windowContent;
        [SerializeField] private Button _reviveButton;
        [SerializeField] private Button _loadSaveButton;

        public void Initialize()
        {
            _windowContent.SetActive(false);
            
            _reviveButton.onClick.AddListener(InvokeOnReviveButtonClicked);
            _loadSaveButton.onClick.AddListener(InvokeOnLoadSaveButtonClicked);
        }
        
        private void OnDestroy()
        {
            _reviveButton.onClick.RemoveListener(InvokeOnReviveButtonClicked);
            _loadSaveButton.onClick.RemoveListener(InvokeOnLoadSaveButtonClicked);
        }
        
        public void Open()
        {
            _windowContent.SetActive(true);
            _windowAnimation.AnimateOpen();
        }

        public void UpdateReviveButtonState(bool state) => 
            _reviveButton.interactable = state;
        
        public async UniTask CloseAsync()
        {
            await _windowAnimation.AnimateCloseAsync();
            _windowContent.SetActive(false);
        }
        
        private void InvokeOnReviveButtonClicked() => 
            OnReviveButtonClicked?.Invoke();

        private void InvokeOnLoadSaveButtonClicked() => 
            OnLoadSaveButtonClicked?.Invoke();
    }
}
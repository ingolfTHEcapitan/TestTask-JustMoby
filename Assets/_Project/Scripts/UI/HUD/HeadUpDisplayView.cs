using System;
using _Project.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayView : MonoBehaviour
    {
        public event Action OnBackToMainMenuButtonClicked;
        
        // V 
        [SerializeField] private Button _backMainMenuButton;
        [field:SerializeField] public Button OpenStatsWindowButton { get; private set; }
        [field:SerializeField] public HealthBarView HealthBarView { get; private set; }
        
        //V
        public void Initialize() => 
            _backMainMenuButton.onClick.AddListener(InvokeOnBackToMainMenuButtonClicked);

        // V
        private void OnDestroy() => 
            _backMainMenuButton.onClick.RemoveListener(InvokeOnBackToMainMenuButtonClicked);

        private void InvokeOnBackToMainMenuButtonClicked() => 
            OnBackToMainMenuButtonClicked?.Invoke();
    }
}
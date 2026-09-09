using System;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayView : MonoBehaviour, IWindow
    {
        public event Action OnWindowDestroy;
        public event Action OnBackToMainMenuButtonClicked;
        
        [SerializeField] private Button _backMainMenuButton;
        [field:SerializeField] public Button OpenStatsWindowButton { get; private set; }
        [field:SerializeField] public HealthBarView HealthBarView { get; private set; }
        
        public void Awake() => 
            _backMainMenuButton.onClick.AddListener(InvokeOnBackToMainMenuButtonClicked);

        public void OnDestroy()
        {
            OnWindowDestroy?.Invoke();
            _backMainMenuButton.onClick.RemoveListener(InvokeOnBackToMainMenuButtonClicked);
        }

        private void InvokeOnBackToMainMenuButtonClicked() => 
            OnBackToMainMenuButtonClicked?.Invoke();
    }
}
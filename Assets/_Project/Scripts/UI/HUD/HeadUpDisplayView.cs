using System;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Common.HealthBar;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayView : MonoBehaviour, IWindow
    {
        public event Action OnWindowDestroy;
        public event Action OnBackToMainMenuButtonClicked;
        public event Action OnOpenStatsButtonClicked;
        
        [SerializeField] private Button _backMainMenuButton;
        [SerializeField] public Button _openStatsWindowButton;
        [field:SerializeField] public HealthBarView HealthBarView { get; private set; }
        
        public void Awake()
        {
            _backMainMenuButton.onClick.AddListener(InvokeOnBackToMainMenuButtonClicked);
            _openStatsWindowButton.onClick.AddListener(InvokeOnOpenStatsButtonClicked);
        }

        public void OnDestroy()
        {
            OnWindowDestroy?.Invoke();
            _backMainMenuButton.onClick.RemoveListener(InvokeOnBackToMainMenuButtonClicked);
            _openStatsWindowButton.onClick.RemoveListener(InvokeOnOpenStatsButtonClicked);
        }

        private void InvokeOnBackToMainMenuButtonClicked() => 
            OnBackToMainMenuButtonClicked?.Invoke();

        private void InvokeOnOpenStatsButtonClicked() => 
            OnOpenStatsButtonClicked?.Invoke();
    }
}
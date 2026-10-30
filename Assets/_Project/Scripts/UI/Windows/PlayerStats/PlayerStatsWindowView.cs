using System;
using _Project.Scripts.Services;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowView : MonoBehaviour, IWindow
    {
        public event Action OnWindowDestroy;
        public event Action OnOpenButtonClicked;
        public event Action OnCloseButtonClicked;
        public event Action OnApplyChangesButtonClicked;
        
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _windowContent;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private TextMeshProUGUI _pointsText;
        [Header("Audio")]
        [SerializeField] private AudioClip _levelUpSound;

        [field: SerializeField] public AudioSource AudioSource { get; private set;}
        [field: SerializeField, Space] public Transform StatsContainer { get; private set; }
        
        private AudioService _audioService;
        private Button _openButton;

        [Inject]
        private void Construct(WindowFactory windowFactory, AudioService audioService) => 
            _audioService = audioService;

        public void Initialize(Button openButton)
        {
            _windowContent.SetActive(false);
            _openButton = openButton;
            _openButton.onClick.AddListener(InvokeOnOpenButtonClicked);
            _closeButton.onClick.AddListener(InvokeOnCloseButtonClicked);
            _applyButton.onClick.AddListener(InvokeOnApplyChangesButtonClicked);
        }

        private void OnDestroy()
        {
            OnWindowDestroy?.Invoke();
            _openButton.onClick.RemoveListener(InvokeOnOpenButtonClicked);
            _closeButton.onClick.RemoveListener(InvokeOnCloseButtonClicked);
            _applyButton.onClick.RemoveListener(InvokeOnApplyChangesButtonClicked);
        }
        
        public void UpdatePointsText(string points) => 
            _pointsText.SetText($"Points {points}");
        
        public void ClearStatsContainer()
        {
            foreach (Transform child in StatsContainer) 
                Destroy(child.gameObject);
        }
        
        public void UpdateStatItem(PlayerStatItemView statItemView, int level, bool canUpgrade)
        {
            statItemView.UpdateLevelText(level);
            statItemView.ToggleUpgradeButton(canUpgrade);
        }
        
        public async UniTask ShowWindowAsync()
        {
            _windowContent.SetActive(true);
            await _windowAnimation.AnimateOpenAsync();
        }

        public async UniTask HideWindowAsync()
        {
            await _windowAnimation.AnimateCloseAsync();
            _windowContent.SetActive(false);
        }
        
        public void PlayLevelUpSound() => 
            _audioService.PlayOneShot(_levelUpSound, AudioSource);
        
        private void InvokeOnOpenButtonClicked() => 
            OnOpenButtonClicked?.Invoke();

        private void InvokeOnCloseButtonClicked() => 
            OnCloseButtonClicked?.Invoke();

        private void InvokeOnApplyChangesButtonClicked() =>
            OnApplyChangesButtonClicked?.Invoke();
    }
}
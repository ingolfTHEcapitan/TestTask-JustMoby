using System;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services.Sound;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowView: MonoBehaviour
    {
        public event Action OnOpenButtonClicked;
        public event Action OnCloseButtonClicked;
        public event Action OnApplyChangesButtonClicked;
        
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _windowContent;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Transform _statsContainer;
        [SerializeField] private TextMeshProUGUI _pointsText;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        
        private IAudioService _audioService;
        private IUIFactory _uiFactory;
        private AudioClip _levelUpSound;
        private Button _openButton;


        [Inject]
        private void Construct(IUIFactory uiFactory, IAudioService audioService)
        {
            _uiFactory = uiFactory;
            _audioService = audioService;
        }

        public void Initialize(Button openButton, AudioClip levelUpSound)
        {
            _windowContent.SetActive(false);
            _levelUpSound = levelUpSound;
            _openButton = openButton;
            _openButton.onClick.AddListener(InvokeOnOpenButtonClicked);
            _closeButton.onClick.AddListener(InvokeOnCloseButtonClicked);
            _applyButton.onClick.AddListener(InvokeOnApplyChangesButtonClicked);
        }

        private void OnDestroy()
        {
            _openButton.onClick.RemoveListener(InvokeOnOpenButtonClicked);
            _closeButton.onClick.RemoveListener(InvokeOnCloseButtonClicked);
            _applyButton.onClick.RemoveListener(InvokeOnApplyChangesButtonClicked);
        }
        
        public void UpdatePointsText(string points) => 
            _pointsText.SetText($"Points {points}");
        
        public async UniTask<PlayerStatItemView> CreatePlayerStatItemAsync(PlayerStatData stat)
        {
            PlayerStatItemView statItem = await _uiFactory.CreatePlayerStatItemViewAsync(_statsContainer);
            statItem.Initialize(stat, _audioSource);
            return statItem;
        }

        public void ClearStatsContainer()
        {
            foreach (Transform child in _statsContainer) 
                Destroy(child.gameObject);
        }
        
        public void UpdateStatItem(PlayerStatItemView statItem, int level, bool canUpgrade)
        {
            statItem.UpdateLevelText(level);
            statItem.ToggleUpgradeButton(canUpgrade);
        }
        
        public void ShowWindow()
        {
            _windowContent.SetActive(true);
            _windowAnimation.AnimateOpen();
        }

        public async UniTask HideWindowAsync()
        {
            await _windowAnimation.AnimateCloseAsync();
            _windowContent.SetActive(false);
        }
        
        public void PlayLevelUpSound() => 
            _audioService.PlayOneShot(_levelUpSound, _audioSource);
        
        private void InvokeOnOpenButtonClicked() => 
            OnOpenButtonClicked?.Invoke();

        private void InvokeOnCloseButtonClicked() => 
            OnCloseButtonClicked?.Invoke();

        private void InvokeOnApplyChangesButtonClicked() => 
            OnApplyChangesButtonClicked?.Invoke();
    }
}
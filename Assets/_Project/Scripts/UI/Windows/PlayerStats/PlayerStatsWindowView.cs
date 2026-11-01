using System;
using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Windows.PlayerStats.StatItem;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowView : MonoBehaviour, IWindow
    {
        public event Action<StatName> OnStatItemUpgradeButtonClicked;
        public event Action OnWindowDestroy;
        public event Action OnCloseButtonClicked;
        public event Action OnApplyChangesButtonClicked;
        
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _windowContent;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private TextMeshProUGUI _pointsText;
        [SerializeField] private Transform _statsContainer;
        [Header("Audio")]
        [SerializeField] private AudioClip _levelUpSound;
        [SerializeField] private AudioSource _audioSource;
        
        private readonly Dictionary<StatName, PlayerStatItemView> _statItemsView = new Dictionary<StatName, PlayerStatItemView>();

        private PlayerStatItemFactory _statItemFactory;
        private ScoreService _scoreService;
        private ISoundService _audioService;

        [Inject]
        private void Construct(WindowFactory windowFactory, PlayerStatItemFactory statItemFactory, ScoreService scoreService, ISoundService audioService)
        {
            _statItemFactory = statItemFactory;
            _scoreService = scoreService;
            _audioService = audioService;
        }

        public void Initialize()
        {
            _windowContent.SetActive(false);
            _closeButton.onClick.AddListener(InvokeOnCloseButtonClicked);
            _applyButton.onClick.AddListener(InvokeOnApplyChangesButtonClicked);
            _scoreService.OnScoreChanged += PlayLevelUpSound;
        }

        private void OnDestroy()
        {
            OnWindowDestroy?.Invoke();
            _closeButton.onClick.RemoveListener(InvokeOnCloseButtonClicked);
            _applyButton.onClick.RemoveListener(InvokeOnApplyChangesButtonClicked);
            _scoreService.OnScoreChanged -= PlayLevelUpSound;
        }
        
        public async UniTask CreateStatItemsAsync(List<(PlayerStatData, PlayerStatConfig)> statsInfos)
        {
            ClearStatItemsContainer();

            foreach ((PlayerStatData data, PlayerStatConfig config) info in statsInfos)
            {
                PlayerStatItemView statItemView = await _statItemFactory.CreateAsync(_statsContainer, info.data, info.config, _audioSource);
                statItemView.OnUpgradeButtonClicked += InvokeOnStatItemUpgradeButtonClicked;
                _statItemsView[info.data.Name] = statItemView;
            }
        }
        
        public void UpdatePointsText(string points) => 
            _pointsText.SetText($"Points {points}");
        
        public void UpdateStatItem(StatName statName, int level, bool canUpgrade)
        {
            if (_statItemsView.TryGetValue(statName, out PlayerStatItemView statItemView))
            {
                statItemView.UpdateLevelText(level);
                statItemView.ToggleUpgradeButton(canUpgrade);
            }
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

        private void PlayLevelUpSound() => 
            _audioService.PlayOneShot(_levelUpSound, _audioSource);

        private void ClearStatItemsContainer()
        {
            foreach (Transform statItem in _statsContainer) 
                Destroy(statItem.gameObject);
            
            foreach (PlayerStatItemView statItemView in _statItemsView.Values)
            {
                if (statItemView)
                {
                    statItemView.OnUpgradeButtonClicked -= InvokeOnStatItemUpgradeButtonClicked;
                    Destroy(statItemView.gameObject);
                }
            }
            
            _statItemsView.Clear();
        }

        private void InvokeOnStatItemUpgradeButtonClicked(StatName statName) => 
            OnStatItemUpgradeButtonClicked?.Invoke(statName);

        private void InvokeOnCloseButtonClicked() => 
            OnCloseButtonClicked?.Invoke();

        private void InvokeOnApplyChangesButtonClicked() =>
            OnApplyChangesButtonClicked?.Invoke();
    }
}
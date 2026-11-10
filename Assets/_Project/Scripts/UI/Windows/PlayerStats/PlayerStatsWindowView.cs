using System;
using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Windows.PlayerStats.Stat;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowView : MonoBehaviour, IWindow
    {
        public event Action<StatName> OnUpgradeButtonClicked;
        public event Action OnWindowDestroy;
        public event Action OnCloseButtonClicked;
        public event Action OnApplyButtonClicked;
        
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _windowContent;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private TextMeshProUGUI _pointsText;
        [SerializeField] private Transform _statContainer;
        [Header("Audio")]
        [SerializeField] private AudioClip _levelUpSound;
        [SerializeField] private AudioSource _audioSource;
        
        private readonly Dictionary<StatName, PlayerStatView> _statViews = new Dictionary<StatName, PlayerStatView>();

        private PlayerStatViewFactory _statFactory;
        private ScoreService _scoreService;
        private ISoundService _audioService;

        [Inject]
        private void Construct(PlayerStatViewFactory statFactory, ScoreService scoreService, ISoundService audioService)
        {
            _statFactory = statFactory;
            _scoreService = scoreService;
            _audioService = audioService;
        }

        public void Initialize()
        {
            ClearStatContainer();
            
            _windowContent.SetActive(false);
            _closeButton.onClick.AddListener(InvokeOnCloseButtonClicked);
            _applyButton.onClick.AddListener(InvokeOnApplyButtonClicked);
            _scoreService.OnScoreChanged += PlayLevelUpSound;
        }

        private void OnDestroy()
        {
            OnWindowDestroy?.Invoke();
            _closeButton.onClick.RemoveListener(InvokeOnCloseButtonClicked);
            _applyButton.onClick.RemoveListener(InvokeOnApplyButtonClicked);
            _scoreService.OnScoreChanged -= PlayLevelUpSound;
        }
        
        public async UniTask CreateStatViewsAsync(List<(PlayerStatData, PlayerStatConfig)> statInfos)
        {
            ClearStatViews();

            foreach ((PlayerStatData data, PlayerStatConfig config) info in statInfos)
            {
                PlayerStatView statView = await _statFactory.CreateAsync(_statContainer, info.data, info.config, _audioSource);
                statView.OnUpgradeButtonClicked += InvokeOnUpgradeButtonClicked;
                _statViews[info.data.Name] = statView;
            }
        }
        
        public void RefreshPointsText(string points) => 
            _pointsText.SetText($"Points {points}");
        
        public void RefreshStatView(StatName statName, int level, bool canUpgrade)
        {
            if (_statViews.TryGetValue(statName, out PlayerStatView statView))
            {
                statView.RefreshLevelText(level);
                statView.ToggleUpgradeButton(canUpgrade);
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

        private void ClearStatViews()
        {
            foreach (PlayerStatView statView in _statViews.Values)
            {
                if (statView)
                {
                    statView.OnUpgradeButtonClicked -= InvokeOnUpgradeButtonClicked;
                    Destroy(statView.gameObject);
                }
            }
            
            _statViews.Clear();
        }

        private void ClearStatContainer()
        {
            foreach (Transform child in _statContainer) 
                Destroy(child.gameObject);
        }

        private void InvokeOnUpgradeButtonClicked(StatName statName) => 
            OnUpgradeButtonClicked?.Invoke(statName);

        private void InvokeOnCloseButtonClicked() => 
            OnCloseButtonClicked?.Invoke();

        private void InvokeOnApplyButtonClicked() =>
            OnApplyButtonClicked?.Invoke();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Logic.Player.PlayerStats.UI
{
    public class PlayerStatsView: MonoBehaviour
    {
        public event Action OnOpenButtonClicked;
        public event Action OnCloseButtonClicked;
        public event Action OnApplyChangesButtonClicked;
        
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _statsWindow;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Transform _statsContainer;
        [SerializeField] private TextMeshProUGUI _pointsText;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        
        private readonly Dictionary<StatName, PlayerStatItemView> _statItems = new Dictionary<StatName, PlayerStatItemView>();
        private IInputService _inputService;
        private Button _openButton;
        private IUIFactory _uiFactory;

        [Inject]
        private void Construct(IInputService inputService, IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            _inputService = inputService;
        }

        public void Initialize(Button openButton)
        {
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
        
        private void Update()
        {
            if (_inputService.IsOpenStatsButtonPressed()) 
                InvokeOnOpenButtonClicked();
        }

        public void UpdatePointsText(string points) => 
            _pointsText.SetText($"Points {points}");

        public async UniTask CreateStatItems(List<PlayerStatData> stats)
        {
            ClearStatItems();
            
            foreach (PlayerStatData stat in stats)
            {
                PlayerStatItemView statItem = await _uiFactory.CreatePlayerStatItem(_statsContainer);
                statItem.Initialize(stat, _audioSource);
                _statItems[stat.Name] = statItem;
            }
        }

        public void UpdateStatItem(StatName statName, int level, bool canUpgrade)
        {
            if (_statItems.TryGetValue(statName, out PlayerStatItemView statItem))
            {
                statItem.UpdateLevelText(level);
                statItem.ToggleUpgradeButton(canUpgrade);
            }
        }

        public List<PlayerStatItemView> GetStatItems() => 
            _statItems.Values.ToList();
        
        public void ShowWindow()
        {
            _statsWindow.SetActive(true);
            _windowAnimation.AnimateOpen();
        }

        public async UniTask HideWindow()
        {
            await _windowAnimation.AnimateClose();
            _statsWindow.SetActive(false);
        }

        private void ClearStatItems()
        {
            foreach (PlayerStatItemView item in _statItems.Values) 
                Destroy(item.gameObject);
            
            _statItems.Clear();
        }

        private void InvokeOnOpenButtonClicked() => 
            OnOpenButtonClicked?.Invoke();

        private void InvokeOnCloseButtonClicked() => 
            OnCloseButtonClicked?.Invoke();

        private void InvokeOnApplyChangesButtonClicked() => 
            OnApplyChangesButtonClicked?.Invoke();
    }
}
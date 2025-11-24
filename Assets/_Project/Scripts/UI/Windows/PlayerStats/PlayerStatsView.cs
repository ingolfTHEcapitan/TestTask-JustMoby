using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services.PlayerInput;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsView: MonoBehaviour
    {
        public event Action OnOpenButtonClicked;
        public event Action OnCloseButtonClicked;
        public event Action OnApplyChangesButtonClicked;
        
        [SerializeField] private GameObject _statsPanel;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Transform _statsContainer;
        [SerializeField] private TextMeshProUGUI _pointsText;
        [SerializeField] private PlayerStatItemView playerStatItemPrefab;
        
        private readonly Dictionary<StatName, PlayerStatItemView> _statItems = new Dictionary<StatName, PlayerStatItemView>();
        private IInputService _inputService;
        private Button _openButton;
        
        [Inject]
        public void Construct(IInputService inputService) => 
            _inputService = inputService;

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

        public void CreateStatItems(List<PlayerStatData> stats)
        {
            ClearStatItems();
            
            foreach (PlayerStatData stat in stats)
            {
                PlayerStatItemView statItem = Instantiate(playerStatItemPrefab, _statsContainer);
                statItem.Initialize(stat);
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
        
        public void ShowPanel() => 
            _statsPanel.SetActive(true);

        public void HidePanel() => 
            _statsPanel.SetActive(false);

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
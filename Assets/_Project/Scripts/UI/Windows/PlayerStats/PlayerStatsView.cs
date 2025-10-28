using System;
using System.Collections.Generic;
using _Project.Scripts.Logic.PlayerStats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsView: MonoBehaviour
    {
        public event Action OnOpenButtonClicked;
        public event Action OnCloseButtonClicked;
        public event Action OnApplyChangesButtonClicked;
        
        public readonly Dictionary<StatName, PlayerStatItemView> StatItems = new Dictionary<StatName, PlayerStatItemView>();
        
        [SerializeField] private GameObject _statsPanel;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Transform _statsContainer;
        [SerializeField] private TextMeshProUGUI _pointsText;
        [SerializeField] private PlayerStatItemView playerStatItemPrefab;
        
        private Button _openButton;
        
        public void Construct(Button openButton) => 
            _openButton = openButton;

        public void Initialize()
        {
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
            if (Input.GetKey(KeyCode.Tab)) 
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
                StatItems[stat.Name] = statItem;
            }
        }

        public void UpdateStatItem(StatName statName, int level, bool canUpgrade)
        {
            if (StatItems.TryGetValue(statName, out PlayerStatItemView statItem))
            {
                statItem.UpdateLevelText(level);
                statItem.ToggleUpgradeButton(canUpgrade);
            }
        }

        public void ShowPanel() => 
            _statsPanel.SetActive(true);

        public void HidePanel() => 
            _statsPanel.SetActive(false);

        private void ClearStatItems()
        {
            foreach (PlayerStatItemView item in StatItems.Values) 
                Destroy(item.gameObject);
            
            StatItems.Clear();
        }

        private void InvokeOnOpenButtonClicked() => 
            OnOpenButtonClicked?.Invoke();

        private void InvokeOnCloseButtonClicked() => 
            OnCloseButtonClicked?.Invoke();

        private void InvokeOnApplyChangesButtonClicked() => 
            OnApplyChangesButtonClicked?.Invoke();
    }
}
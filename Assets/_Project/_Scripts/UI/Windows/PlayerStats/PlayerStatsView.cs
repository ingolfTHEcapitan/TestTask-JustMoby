using System.Collections.Generic;
using _Project._Scripts.Logic.PlayerStats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsView: MonoBehaviour
    {
        [SerializeField] private GameObject _statsPanel;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Transform _statsContainer;
        [SerializeField] private TextMeshProUGUI _pointsText;
        [SerializeField] private PlayerStatItemView playerStatItemPrefab;
        
        private readonly Dictionary<StatName, PlayerStatItemView> _statItems = new Dictionary<StatName, PlayerStatItemView>();
        
        private PlayerStatsPresenter _presenter;
        private Button _openButton;
        
        public void Construct(PlayerStatsPresenter presenter, Button openButton)
        {
            _presenter = presenter;
            _openButton = openButton;
        }

        private void Start()
        {
            _openButton.onClick.AddListener(OnOpenButtonClicked);
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            _applyButton.onClick.AddListener(OnApplyChangesButtonClicked);
        }

        private void OnDestroy()
        {
            _openButton.onClick.RemoveListener(OnOpenButtonClicked);
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            _applyButton.onClick.RemoveListener(OnApplyChangesButtonClicked);
        }
        
        private void Update()
        {
            if (Input.GetKey(KeyCode.Tab)) 
                OnOpenButtonClicked();
        }

        public void UpdatePointsText(string points) => 
            _pointsText.SetText($"Points {points}");

        public void CreateStatItems(List<PlayerStatData> stats)
        {
            ClearStatItems();
            
            foreach (PlayerStatData stat in stats)
            {
                PlayerStatItemView statItem = Instantiate(playerStatItemPrefab, _statsContainer);
                statItem.Construct(_presenter);
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

        private void OnOpenButtonClicked() => 
            _presenter.Open();

        private void OnCloseButtonClicked() => 
            _presenter.Close();

        private void OnApplyChangesButtonClicked() => 
            _presenter.ApplyChanges();
    }
}
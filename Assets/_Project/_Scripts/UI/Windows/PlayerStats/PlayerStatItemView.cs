using _Project._Scripts.Logic.PlayerStats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI.Windows.PlayerStats
{
    internal class PlayerStatItemView: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Image _iconFrame;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _upgradeButton;
        
        private StatName _statName;
        private PlayerStatsPresenter _presenter;
        
        public void Construct(PlayerStatsPresenter presenter) => 
            _presenter = presenter;

        private void OnDestroy() => 
            _upgradeButton.onClick.RemoveListener(OnUpgradeButtonClick);

        public void Initialize(PlayerStatData stat)
        {
            _statName = stat.Name;
            _nameText.SetText(_statName.ToString());
            _iconFrame.sprite = stat.IconFrame;
            _icon.sprite = stat.Icon;
            _upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        }

        public void UpdateLevelText(int level) => 
            _levelText.SetText(level.ToString());
        
        public void ToggleUpgradeButton(bool enable) => 
            _upgradeButton.interactable = enable;

        private void OnUpgradeButtonClick() => 
            _presenter.UpgradeStat(_statName);
    }
}
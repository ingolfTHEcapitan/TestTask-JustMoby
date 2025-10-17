using _Project._Scripts.Logic.StatSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI.Windows.UpgradeStats
{
    internal class UpgradeStatItem: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Button _upgradeButton;
        
        private PlayerStatsSystem _statsSystem;
        private PlayerStat _stat;

        public void Construct(PlayerStatsSystem statsSystem, PlayerStat stat)
        {
            _statsSystem = statsSystem;
            _stat = stat;
        }

        public void Initialize()
        {
            _nameText.SetText(_stat.Name.ToString());
            _levelText.SetText(_stat.Level.ToString());
            _upgradeButton.onClick.AddListener(TryUpgradeStat);
        }

        private void OnDestroy() => 
            _upgradeButton.onClick.RemoveListener(TryUpgradeStat);

        public void HandleUpgradeButton() =>
            ToggleUpgradeButton(_statsSystem.CanUpgrade(_stat.Name));

        public void ResetStatLevel() => 
            _levelText.SetText(_stat.Level.ToString());

        private void TryUpgradeStat()
        {
            _statsSystem.UpgradeStat(statName: _stat.Name);
            _levelText.SetText(sourceText: _stat.PreviewLevel.ToString());

            if (!_statsSystem.CanUpgrade(statName: _stat.Name)) 
                ToggleUpgradeButton(enable: false);
        }

        private void ToggleUpgradeButton(bool enable) => 
            _upgradeButton.interactable = enable;
    }
}
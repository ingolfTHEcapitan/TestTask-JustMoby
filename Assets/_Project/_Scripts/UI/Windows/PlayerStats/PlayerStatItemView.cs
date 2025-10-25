using System;
using _Project._Scripts.Logic.PlayerStats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatItemView: MonoBehaviour
    {
        public event Action<StatName> OnUpgradeButtonClicked;
        
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Image _iconFrame;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _upgradeButton;
        
        private StatName _statName;
        
        private void OnDestroy() => 
            _upgradeButton.onClick.RemoveListener(InvokeOnUpgradeButtonClicked);

        public void Initialize(PlayerStatData stat)
        {
            _upgradeButton.onClick.AddListener(InvokeOnUpgradeButtonClicked);
            
            _statName = stat.Name;
            _nameText.SetText(_statName.ToString());
            _iconFrame.sprite = stat.IconFrame;
            _icon.sprite = stat.Icon;
        }

        public void UpdateLevelText(int level) => 
            _levelText.SetText(level.ToString());
        
        public void ToggleUpgradeButton(bool enable) => 
            _upgradeButton.interactable = enable;

        private void InvokeOnUpgradeButtonClicked() => 
            OnUpgradeButtonClicked?.Invoke(_statName);
    }
}
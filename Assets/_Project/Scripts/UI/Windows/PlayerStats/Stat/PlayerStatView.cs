using System;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.PlayerStats.Stat
{
    public class PlayerStatView : MonoBehaviour
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

        public void Initialize(PlayerStatData data, Sprite iconFrame, Sprite icon, AudioSource audioSource)
        {
            _upgradeButton.onClick.AddListener(InvokeOnUpgradeButtonClicked);
            
            _statName = data.Name;
            _nameText.SetText(_statName.ToString());
            _iconFrame.sprite = iconFrame;
            _icon.sprite = icon;

            if (_upgradeButton.TryGetComponent(out ButtonSoundEffect soundEffect)) 
                soundEffect.Initialize(audioSource);
            else
                Debug.LogWarning($"[PlayerStatView] No ButtonSoundEffect component attached to {_upgradeButton.name}");
        }

        public void RefreshLevelText(int level) => 
            _levelText.SetText(level.ToString());
        
        public void ToggleUpgradeButton(bool interactable) => 
            _upgradeButton.interactable = interactable;

        private void InvokeOnUpgradeButtonClicked() => 
            OnUpgradeButtonClicked?.Invoke(_statName);
    }
}
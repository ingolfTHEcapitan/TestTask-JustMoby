using System.Collections.Generic;
using _Project._Scripts.Player.StatSystem;
using _Project._Scripts.Services.GamePause;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI
{
    public class UpgradeStatsWindow: MonoBehaviour
    {
        [SerializeField] private GameObject _statsPanel;
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Transform _statsContainer;
        [SerializeField] private TextMeshProUGUI _pointsText;
        [SerializeField] private UpgradeStatItem _upgradeStatItemPrefab;
        
        private readonly List<UpgradeStatItem> _items = new List<UpgradeStatItem>();
        
        private bool _isOpen;
        private PlayerStatsSystem _statsSystem;
        private IGamePauseService _pauseService;

        private void Start()
        {
            _openButton.onClick.AddListener(Open);
            _closeButton.onClick.AddListener(Close);
            _applyButton.onClick.AddListener(ApplyChanges);
            _statsSystem.OnStatsChanged += OnStatsChanged;
        }

        private void OnDestroy()
        {
            _openButton.onClick.RemoveListener(Open);
            _closeButton.onClick.RemoveListener(Close);
            _applyButton.onClick.RemoveListener(ApplyChanges);
            _statsSystem.OnStatsChanged -= OnStatsChanged;
        }

        public void Construct(PlayerStatsSystem statsSystem, IGamePauseService gamePauseService)
        {
            _statsSystem = statsSystem;
            _pauseService = gamePauseService;
        }

        public void Initialize()
        {
            _items.Clear();

            foreach (PlayerStat stat in _statsSystem.Stats.Values)
            {
                UpgradeStatItem item = Instantiate(_upgradeStatItemPrefab, _statsContainer);
                item.Construct(_statsSystem, stat);
                item.Initialize();
                _items.Add(item);
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Tab)) 
                Open();
        }

        private void Open()
        {
            if (_isOpen)
                return;
            
            _isOpen = true;
            _statsPanel.SetActive(true);
            _pauseService.SetPaused(true);
            OnOpen();
        }

        private void OnOpen()
        {
            foreach (UpgradeStatItem item in _items)
            {
                UpgradePointsText();
                item.HandleUpgradeButton();
                item.ResetStatLevel();
            }
        }

        private void UpgradePointsText() => 
            _pointsText.SetText(_statsSystem.UpgradePoints.ToString());

        private void Close()
        {
            _isOpen = false;
            _statsPanel.SetActive(false);
            _pauseService.SetPaused(false);
            _statsSystem.DiscardPreviewChanges();
        }

        private void ApplyChanges()
        {
            _statsSystem.ApplyChanges();
            Close();
        }

        private void OnStatsChanged() => 
            UpgradePointsText();
    }
}
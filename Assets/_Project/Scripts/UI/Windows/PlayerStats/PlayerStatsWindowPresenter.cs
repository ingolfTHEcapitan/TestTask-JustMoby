using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.PlayerStats.Data;
using _Project.Scripts.Services.PlayerInput;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowPresenter : IDisposable
    {
        private readonly IInputService _inputService;
        private readonly PlayerStatsWindowModel _model;
        
        private readonly Dictionary<StatName, PlayerStatItemView> _statItemsView = new Dictionary<StatName, PlayerStatItemView>();
        
        private PlayerStatsWindowView _windowView;
        private PlayerDeath _playerDeath;
        private bool _isOpen;

        public PlayerStatsWindowPresenter(IInputService inputService, PlayerStatsWindowModel model)
        {
            _inputService = inputService;
            _model = model;
        }
        
        public void Construct(PlayerStatsWindowView view, PlayerDeath playerDeath)
        {
            _windowView = view;
            _playerDeath = playerDeath;
        }
        
        public async UniTask InitializeAsync()
        {
            _model.OnStatsChanged += UpdateAllStatItems;
            _windowView.OnOpenButtonClicked += Open;
            _windowView.OnCloseButtonClicked += Close;
            _windowView.OnApplyChangesButtonClicked += ApplyChanges;
            _inputService.OnOpenStatsButtonPressed += Open;
            
            await CreateStatItemsAsync(_model.GetStatValues());
            
            foreach (PlayerStatItemView statItemView in GetStatItems())
                statItemView.OnUpgradeButtonClicked += UpgradeStatItem;
        }

        public void Dispose()
        {
            _model.OnStatsChanged -= UpdateAllStatItems;
            _windowView.OnOpenButtonClicked -= Open;
            _windowView.OnCloseButtonClicked -= Close;
            _windowView.OnApplyChangesButtonClicked -= ApplyChanges;
            
            foreach (PlayerStatItemView statItemView in GetStatItems())
                statItemView.OnUpgradeButtonClicked -= UpgradeStatItem;
        }
        
        private async UniTask CreateStatItemsAsync(List<PlayerStatData> stats)
        {
            ClearStatItems();
            
            foreach (PlayerStatData stat in stats)
            {
                PlayerStatItemView statItemView = await _windowView.CreatePlayerStatItemAsync(stat);
                _statItemsView[stat.Name] = statItemView;
            }
        }

        private void UpgradeStatItem(StatName statName)
        {
            _model.UpgradeStat(statName);
            UpdateStatItem(statName);
        }

        private void Open()
        {
            if (_isOpen || _playerDeath.IsDead)
                return;
            
            _isOpen = true;
            _model.SetPaused(true);
            _windowView.ShowWindow();
            UpdateAllStatItems();
        }

        private async void Close()
        {
            _isOpen = false;
            _model.SetPaused(false);
            await _windowView.HideWindowAsync();
            _model.DiscardPreviewChanges();
        }

        private void ApplyChanges()
        {
            _model.ApplyChanges();
            Close();
        }

        private void UpdateAllStatItems()
        {
            _windowView.UpdatePointsText(_model.UpgradePoints.ToString());
            
            foreach (var stat in _model.GetStatValues())
                UpdateStatItem(stat.Name);
        }

        private void UpdateStatItem(StatName statName)
        {
            PlayerStatData stat = _model.GetStat(statName);
            bool canUpgrade = _model.CanUpgrade(statName);
           
            if (_statItemsView.TryGetValue(statName, out PlayerStatItemView statItemView)) 
                _windowView.UpdateStatItem(statItemView, stat.PreviewLevel, canUpgrade);
        }

        private void ClearStatItems()
        {
            foreach (PlayerStatItemView statItemView in _statItemsView.Values) 
                Object.Destroy(statItemView.gameObject);
            
            _statItemsView.Clear();
        }
        
        private List<PlayerStatItemView> GetStatItems() => 
            _statItemsView.Values.ToList();
    }
}
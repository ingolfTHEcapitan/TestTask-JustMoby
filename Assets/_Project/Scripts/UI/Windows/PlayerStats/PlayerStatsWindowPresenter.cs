using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.UpgradePoints;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowPresenter : IDisposable
    {
        private readonly IInputService _inputService;
        private readonly IUpgradePointsService _pointsService;
        private readonly PlayerStatsWindowModel _model;
        private readonly PlayerStatsWindowView _view;
        private readonly CursorController _cursorController;

        private readonly Dictionary<StatName, PlayerStatItemView> _statItemsView = new Dictionary<StatName, PlayerStatItemView>();

        private bool _isOpen;

        public PlayerStatsWindowPresenter( PlayerStatsWindowModel model, PlayerStatsWindowView view,
            IInputService inputService, IUpgradePointsService pointsService, CursorController cursorController)
        {
            _model = model;
            _view = view;
            _inputService = inputService;
            _pointsService = pointsService;
            _cursorController = cursorController;
        }
        
        public async UniTask InitializeAsync()
        {
            await _model.Initialize();
            
            _model.OnStatsChanged += UpdateAllStatItems;
            _view.OnOpenButtonClicked += Open;
            _view.OnCloseButtonClicked += Close;
            _view.OnApplyChangesButtonClicked += ApplyChanges;
            _inputService.OnOpenStatsButtonPressed += Open;
            _pointsService.OnPointAdded += _view.PlayLevelUpSound;
            
            await CreateStatItemsAsync();
            
            foreach (PlayerStatItemView statItemView in GetStatItems())
                statItemView.OnUpgradeButtonClicked += UpgradeStatItem;
        }

        public void Dispose()
        {
            _model.OnStatsChanged -= UpdateAllStatItems;
            _view.OnOpenButtonClicked -= Open;
            _view.OnCloseButtonClicked -= Close;
            _view.OnApplyChangesButtonClicked -= ApplyChanges;
            _pointsService.OnPointAdded -= _view.PlayLevelUpSound;
            
            foreach (PlayerStatItemView statItemView in GetStatItems())
                statItemView.OnUpgradeButtonClicked -= UpgradeStatItem;
            
            _model.Dispose();
        }
        
        private async UniTask CreateStatItemsAsync()
        {
            _view.ClearStatsContainer();
            ClearStatItems();
            
            foreach (PlayerStatData stat in _model.GetStats())
            {
                PlayerStatItemView statItemView = await _view.CreatePlayerStatItemAsync(stat);
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
            if (_isOpen || _model.IsPlayerDead)
                return;
            
            _isOpen = true;
            _model.SetPaused(true);
            _cursorController.SetCursorVisible(true);
            _view.ShowWindow();
            UpdateAllStatItems();
        }

        private async void Close()
        {
            _isOpen = false;
            _model.SetPaused(false);
            _cursorController.SetCursorVisible(false);
            await _view.HideWindowAsync();
            _model.DiscardPreviewChanges();
        }

        private void ApplyChanges()
        {
            _model.ApplyChanges();
            Close();
        }

        private void UpdateAllStatItems()
        {
            _view.UpdatePointsText(_model.UpgradePoints.ToString());
            
            foreach (var stat in _model.GetStats())
                UpdateStatItem(stat.Name);
        }

        private void UpdateStatItem(StatName statName)
        {
            PlayerStatData stat = _model.GetStat(statName);
            bool canUpgrade = _model.CanUpgrade(statName);
           
            if (_statItemsView.TryGetValue(statName, out PlayerStatItemView statItemView)) 
                _view.UpdateStatItem(statItemView, stat.PreviewLevel, canUpgrade);
        }

        private void ClearStatItems()
        {
            foreach (PlayerStatItemView statItemView in _statItemsView.Values)
                if (statItemView)
                    Object.Destroy(statItemView.gameObject);
            
            _statItemsView.Clear();
        }
        
        private List<PlayerStatItemView> GetStatItems() => 
            _statItemsView.Values.ToList();
    }
}
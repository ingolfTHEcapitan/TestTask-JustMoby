using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using _Project.Scripts.Logic.Player.PlayerStats.UI;
using _Project.Scripts.Services.PlayerInput;
using Cysharp.Threading.Tasks;
using Zenject;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Logic.Player.PlayerStats
{
    public class PlayerStatsPresenter : IDisposable, ITickable
    {
        private readonly IInputService _inputService;
        private readonly PlayerStatsModel _model;
        
        private readonly Dictionary<StatName, PlayerStatItemView> _statItemsView = new Dictionary<StatName, PlayerStatItemView>();
        
        private PlayerStatsView _view;
        private PlayerDeath _playerDeath;
        private bool _isOpen;

        public PlayerStatsPresenter(IInputService inputService, PlayerStatsModel model, PlayerStatsData statsData)
        {
            _inputService = inputService;
            _model = model;
        }
        
        public void Construct(PlayerStatsView view, PlayerDeath playerDeath)
        {
            _view = view;
            _playerDeath = playerDeath;
        }
        
        public async UniTask InitializeAsync()
        {
            _model.OnStatsChanged += UpdateAllStatItems;
            _view.OnOpenButtonClicked += Open;
            _view.OnCloseButtonClicked += Close;
            _view.OnApplyChangesButtonClicked += ApplyChanges;
            
            await CreateStatItemsAsync(_model.GetStatValues());
            
            foreach (PlayerStatItemView statItemView in GetStatItems())
                statItemView.OnUpgradeButtonClicked += UpgradeStatItem;
        }

        public void Dispose()
        {
            _model.OnStatsChanged -= UpdateAllStatItems;
            _view.OnOpenButtonClicked -= Open;
            _view.OnCloseButtonClicked -= Close;
            _view.OnApplyChangesButtonClicked -= ApplyChanges;
            
            foreach (PlayerStatItemView statItemView in GetStatItems())
                statItemView.OnUpgradeButtonClicked -= UpgradeStatItem;
        }

        public void Tick()
        {
            if (_inputService.IsOpenStatsButtonPressed()) 
                Open();
        }

        private async UniTask CreateStatItemsAsync(List<PlayerStatData> stats)
        {
            ClearStatItems();
            
            foreach (PlayerStatData stat in stats)
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
            if (_isOpen || _playerDeath.IsDead)
                return;
            
            _isOpen = true;
            _model.SetPaused(true);
            _view.ShowWindow();
            UpdateAllStatItems();
        }

        private async void Close()
        {
            _isOpen = false;
            _model.SetPaused(false);
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
            
            foreach (var stat in _model.GetStatValues())
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
                Object.Destroy(statItemView.gameObject);
            
            _statItemsView.Clear();
        }
        
        private List<PlayerStatItemView> GetStatItems() => 
            _statItemsView.Values.ToList();
    }
}
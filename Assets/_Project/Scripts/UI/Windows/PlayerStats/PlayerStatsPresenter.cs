using System;
using _Project.Scripts.Infrastructure.Services.GamePause;
using _Project.Scripts.Logic.PlayerStats;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsPresenter: IDisposable
    {
        private readonly PlayerStatsView _view;
        private readonly PlayerStatsModel _model;
        private readonly IGamePauseService _pauseService;
        private bool _isOpen;

        public PlayerStatsPresenter(PlayerStatsView view, PlayerStatsModel model, IGamePauseService pauseService)
        {
            _view = view;
            _model = model;
            _pauseService = pauseService;
        }

        public void Initialize()
        {
            _model.OnStatsChanged += OnStatsChanged;
            _view.OnOpenButtonClicked += Open;
            _view.OnCloseButtonClicked += Close;
            _view.OnApplyChangesButtonClicked += ApplyChanges;
            
            _view.CreateStatItems(_model.GetStats());
            
            foreach (PlayerStatItemView statItemView in _view.StatItems.Values) 
                statItemView.OnUpgradeButtonClicked += UpgradeStatItem;
        }

        public void Dispose()
        {
            _model.OnStatsChanged -= OnStatsChanged;
            _view.OnOpenButtonClicked -= Open;
            _view.OnCloseButtonClicked -= Close;
            _view.OnApplyChangesButtonClicked -= ApplyChanges;
            
            foreach (PlayerStatItemView statItemView in _view.StatItems.Values) 
                statItemView.OnUpgradeButtonClicked -= UpgradeStatItem;
        }

        private void UpgradeStatItem(StatName statName)
        {
            _model.UpgradeStat(statName);
            UpdateStatItem(statName);
        }

        private void Open()
        {
            if(_isOpen)
                return;
            
            _isOpen = true;
            _view.ShowPanel();
            _pauseService.SetPaused(true);
            OnStatsChanged();
        }

        private void Close()
        {
            _isOpen = false;
            _view.HidePanel();
            _pauseService.SetPaused(false);
            _model.DiscardPreviewChanges();
        }

        private void ApplyChanges()
        {
            _model.ApplyChanges();
            Close();
        }

        private void OnStatsChanged()
        {
            _view.UpdatePointsText(_model.UpgradePoints.ToString());
            UpdateAllStatItems();
        }

        private void UpdateAllStatItems()
        {
            foreach (var stat in _model.Stats.Values)
                UpdateStatItem(stat.Name);
        }

        private void UpdateStatItem(StatName statName)
        {
            PlayerStatData stat = _model.Stats[statName];
            bool canUpgrade = _model.CanUpgrade(statName);
            _view.UpdateStatItem(statName, stat.PreviewLevel, canUpgrade);
        }
    }
}
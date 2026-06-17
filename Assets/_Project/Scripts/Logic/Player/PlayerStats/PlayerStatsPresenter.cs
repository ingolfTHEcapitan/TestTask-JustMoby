using System;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using _Project.Scripts.Logic.Player.PlayerStats.UI;
using _Project.Scripts.Services.GamePause;

namespace _Project.Scripts.Logic.Player.PlayerStats
{
    public class PlayerStatsPresenter : IDisposable
    {
        private readonly PlayerStatsView _view;
        private readonly PlayerStatsModel _model;
        private readonly IGamePauseService _pauseService;
        private readonly PlayerDeath _playerDeath;
        private bool _isOpen;
        private PlayerStatsData _statsData;

        public PlayerStatsPresenter(PlayerStatsView view, PlayerStatsModel model, PlayerStatsData statsData,
            IGamePauseService pauseService, PlayerDeath playerDeath)
        {
            _view = view;
            _model = model;
            _statsData = statsData;
            _pauseService = pauseService;
            _playerDeath = playerDeath;
        }

        public void Initialize()
        {
            _model.OnStatsChanged += UpdateStatItems;
            _view.OnOpenButtonClicked += Open;
            _view.OnCloseButtonClicked += Close;
            _view.OnApplyChangesButtonClicked += ApplyChanges;
            
            _view.CreateStatItems(_statsData.GetStats());
            
            foreach (PlayerStatItemView statItemView in _view.GetStatItems())
                statItemView.OnUpgradeButtonClicked += UpgradeStatItem;
        }

        public void Dispose()
        {
            _model.OnStatsChanged -= UpdateStatItems;
            _view.OnOpenButtonClicked -= Open;
            _view.OnCloseButtonClicked -= Close;
            _view.OnApplyChangesButtonClicked -= ApplyChanges;
            
            foreach (PlayerStatItemView statItemView in _view.GetStatItems())
                statItemView.OnUpgradeButtonClicked -= UpgradeStatItem;
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
            _view.ShowPanel();
            _pauseService.SetPaused(true);
            UpdateStatItems();
        }

        private void Close()
        {
            _isOpen = false;
            _view.HidePanel();
            _pauseService.SetPaused(false);
            _model.DiscardPreviewChanges();
        }

        private void  ApplyChanges()
        {
            _model.ApplyChanges();
            Close();
        }

        private void UpdateStatItems()
        {
            _view.UpdatePointsText(_model.UpgradePoints.ToString());
            UpdateAllStatItems();
        }

        private void UpdateAllStatItems()
        {
            foreach (var stat in _statsData.GetStats())
                UpdateStatItem(stat.Name);
        }

        private void UpdateStatItem(StatName statName)
        {
            PlayerStatData stat = _statsData.GetStat(statName);
            bool canUpgrade = _model.CanUpgrade(statName);
            _view.UpdateStatItem(statName, stat.PreviewLevel, canUpgrade);
        }
    }
}
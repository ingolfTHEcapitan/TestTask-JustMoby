using System;
using System.Collections.Generic;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Logic.PlayerStats;

namespace _Project._Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsPresenter: IDisposable
    {
        private PlayerStatsView _view;
        private PlayerStatsModel _model;
        private IGamePauseService _pauseService;
        private bool _isOpen;

        public PlayerStatsPresenter(PlayerStatsView view, PlayerStatsModel model, IGamePauseService pauseService)
        {
            _view = view;
            _model = model;
            _pauseService = pauseService;
        }

        public void Initialize(List<PlayerStatData> stats)
        {
            _model.OnStatsChanged += OnStatsChanged;
            _view.CreateStatItems(stats);
        }

        public void Dispose() => 
            _model.OnStatsChanged -= OnStatsChanged;

        public void Open()
        {
            if(_isOpen)
                return;
            
            _isOpen = true;
            _view.ShowPanel();
            _pauseService.SetPaused(true);
            OnStatsChanged();
        }

        public void Close()
        {
            _isOpen = false;
            _view.HidePanel();
            _pauseService.SetPaused(false);
            _model.DiscardPreviewChanges();
        }

        public void ApplyChanges()
        {
            _model.ApplyChanges();
            Close();
        }

        public void UpgradeStat(StatName statName)
        {
            _model.UpgradeStat(statName);
            UpdateStatItem(statName);
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
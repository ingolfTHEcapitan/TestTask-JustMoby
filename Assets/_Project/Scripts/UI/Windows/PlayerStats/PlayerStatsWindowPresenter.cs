using System;
using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowPresenter
    {
        private readonly IInputService _inputService;
        private readonly PlayerStatsWindowModel _model;
        private readonly PlayerStatsWindowView _view;
        private readonly CursorController _cursorController;
        
        private bool _isOpen;

        public PlayerStatsWindowPresenter(PlayerStatsWindowModel model, PlayerStatsWindowView view, 
            IInputService inputService, CursorController cursorController)
        {
            _model = model;
            _view = view;
            _inputService = inputService;
            _cursorController = cursorController;
        }

        public async UniTask InitializeAsync()
        {
            _model.Initialize();
            _view.Initialize();

            _view.OnWindowDestroy += CleanUp;
            _view.OnCloseButtonClicked += Close;
            _view.OnApplyButtonClicked += ApplyStatUpgradesAndClose;
            _view.OnUpgradeButtonClicked += PreviewStatUpgrade;
            _model.OnStatsChanged += RefreshAllStatViews;
            _inputService.OnOpenStatsButtonPressed += Open;

            await CreateStatViewsAsync();
        }

        private void CleanUp()
        {
            _view.OnWindowDestroy -= CleanUp;
            _view.OnCloseButtonClicked -= Close;
            _view.OnApplyButtonClicked -= ApplyStatUpgradesAndClose;
            _view.OnUpgradeButtonClicked -= PreviewStatUpgrade;
            _model.OnStatsChanged -= RefreshAllStatViews;
            _inputService.OnOpenStatsButtonPressed -= Open;
            
            _model.Dispose();
        }

        public async void Open()
        {
            try
            {
                if (_isOpen || _model.IsPlayerDead)
                    return;
            
                _isOpen = true;
                _model.SetPaused(true);
                _cursorController.SetCursorVisible(true);
                RefreshAllStatViews();
                await _view.ShowWindowAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void Close() => 
            CloseAsync().Forget(Debug.LogError);

        private async void ApplyStatUpgradesAndClose()
        {
            try
            {
                await _model.ApplyStatUpgradesAsync();
                await CloseAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private async UniTask CloseAsync()
        {
            await _view.HideWindowAsync();
            _model.SetPaused(false);
            _cursorController.SetCursorVisible(false);
                
            _model.DiscardPreviewChanges();
            _isOpen = false;
        }

        private void PreviewStatUpgrade(StatName statName)
        {
            _model.PreviewStatUpgrade(statName);
            RefreshStatView(statName);
        }

        private void RefreshAllStatViews()
        {
            foreach (PlayerStatData stat in _model.GetStats())
                RefreshStatView(stat.Name, stat);
        }

        private void RefreshStatView(StatName statName, PlayerStatData stat = null)
        {
            stat ??= _model.GetStat(statName);
            bool canUpgrade = _model.CanUpgrade(statName);
            
           _view.RefreshStatView(statName, stat.PreviewLevel, canUpgrade);
           _view.RefreshPointsText(_model.UpgradePoints.ToString());
        }
        
        private async UniTask CreateStatViewsAsync()
        {
            var statInfos = new List<(PlayerStatData, PlayerStatConfig)>();
            
            foreach (PlayerStatData data in _model.GetStats())
            {
                PlayerStatConfig config = _model.FindStatConfigByName(data.Name);
                statInfos.Add((data, config));
            }
            
            await _view.CreateStatViewsAsync(statInfos);
        }
    }
}
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

        private void CleanUp()
        {
            _view.OnWindowDestroy -= CleanUp;
            _view.OnCloseButtonClicked -= Close;
            _view.OnApplyChangesButtonClicked -= ApplyChangesAndClose;
            _view.OnStatItemUpgradeButtonClicked -= UpgradeStatItem;
            _model.OnStatsChanged -= UpdateAllStatItems;
            _inputService.OnOpenStatsButtonPressed -= Open;
            
            _model.Dispose();
        }

        public async UniTask InitializeAsync()
        {
            _model.Initialize();
            _view.Initialize();

            _view.OnWindowDestroy += CleanUp;
            _view.OnCloseButtonClicked += Close;
            _view.OnApplyChangesButtonClicked += ApplyChangesAndClose;
            _view.OnStatItemUpgradeButtonClicked += UpgradeStatItem;
            _model.OnStatsChanged += UpdateAllStatItems;
            _inputService.OnOpenStatsButtonPressed += Open;

            await CreateStatItemsAsync();
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
                UpdateAllStatItems();
                await _view.ShowWindowAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private async void Close()
        {
            try
            {
                await _view.HideWindowAsync();
                _model.SetPaused(false);
                _cursorController.SetCursorVisible(false);
                
                _model.DiscardPreviewChanges();
                _isOpen = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        private async void ApplyChangesAndClose()
        {
            await _model.ApplyChangesAsync();
            Close();
        }

        private void UpgradeStatItem(StatName statName)
        {
            _model.UpgradeStat(statName);
            UpdateStatItem(statName);
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
           _view.UpdateStatItem(statName, stat.PreviewLevel, canUpgrade);
        }
        
        private async UniTask CreateStatItemsAsync()
        {
            var statsInfos = new List<(PlayerStatData, PlayerStatConfig)>();
            
            foreach (PlayerStatData data in _model.GetStats())
            {
                PlayerStatConfig config = _model.FindStatConfigByName(data.Name);
                statsInfos.Add((data, config));
            }
            
            await _view.CreateStatItemsAsync(statsInfos);
        }
    }
}
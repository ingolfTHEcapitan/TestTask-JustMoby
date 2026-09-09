using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.Score;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.UI.Windows.PlayerStats
{
    public class PlayerStatsWindowPresenter
    {
        private readonly IInputService _inputService;
        private readonly IScoreService _scoreService;
        private readonly IUIFactory _uiFactory;
        private readonly PlayerStatsWindowModel _model;
        private readonly PlayerStatsWindowView _view;
        private readonly CursorController _cursorController;

        private readonly Dictionary<StatName, PlayerStatItemView> _statItemsView = new Dictionary<StatName, PlayerStatItemView>();

        private bool _isOpen;

        public PlayerStatsWindowPresenter( PlayerStatsWindowModel model, PlayerStatsWindowView view, IUIFactory uiFactory,
            IInputService inputService, IScoreService scoreService, CursorController cursorController)
        {
            _model = model;
            _view = view;
            _uiFactory = uiFactory;
            _inputService = inputService;
            _scoreService = scoreService;
            _cursorController = cursorController;
        }
        
        public async UniTask InitializeAsync()
        {
            _model.Initialize();

            _view.OnWindowDestroy += CleanUp;
            _view.OnOpenButtonClicked += Open;
            _view.OnCloseButtonClicked += Close;
            _view.OnApplyChangesButtonClicked += ApplyChanges;
            _model.OnStatsChanged += UpdateAllStatItems;
            _inputService.OnOpenStatsButtonPressed += Open;
            _scoreService.OnScoreChanged += _view.PlayLevelUpSound;
            
            await CreateStatItemsAsync();
            
            foreach (PlayerStatItemView statItemView in GetStatItems())
                statItemView.OnUpgradeButtonClicked += UpgradeStatItem;
        }

        private void CleanUp()
        {
            _view.OnWindowDestroy -= CleanUp;
            _view.OnOpenButtonClicked -= Open;
            _view.OnCloseButtonClicked -= Close;
            _view.OnApplyChangesButtonClicked -= ApplyChanges;
            _model.OnStatsChanged -= UpdateAllStatItems;
            _inputService.OnOpenStatsButtonPressed -= Open;
            _scoreService.OnScoreChanged -= _view.PlayLevelUpSound;
            
            foreach (PlayerStatItemView statItemView in GetStatItems())
                statItemView.OnUpgradeButtonClicked -= UpgradeStatItem;
            
            _model.Dispose();
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

        private async UniTask CreateStatItemsAsync()
        {
            _view.ClearStatsContainer();
            ClearStatItems();
            
            foreach (PlayerStatData stat in _model.GetStats())
            {
                PlayerStatConfig statConfig = _model.FindStatConfigByName(stat.Name);
                PlayerStatItemView statItemView = await CreatePlayerStatItemAsync(stat, statConfig);
                _statItemsView[stat.Name] = statItemView;
            }
        }

        private async UniTask<PlayerStatItemView> CreatePlayerStatItemAsync(PlayerStatData stat, PlayerStatConfig statConfig)
        {
            PlayerStatItemView statItem = await _uiFactory.CreatePlayerStatItemViewAsync(_view.StatsContainer);
            
            Sprite iconFrame = await _uiFactory.LoadSpriteAsync(statConfig.IconFrameAddress);
            Sprite icon = await _uiFactory.LoadSpriteAsync(statConfig.IconAddress);
            
            statItem.Initialize(stat, iconFrame, icon, _view.AudioSource);
            return statItem;
        }
        
        private void UpgradeStatItem(StatName statName)
        {
            _model.UpgradeStat(statName);
            UpdateStatItem(statName);
        }

        private async void Open()
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

        private async void ApplyChanges()
        {
            try
            {
                await _view.HideWindowAsync();
                _model.SetPaused(false);
                _cursorController.SetCursorVisible(false);

                await _model.ApplyChangesAsync();
                _model.DiscardPreviewChanges();
                _isOpen = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
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
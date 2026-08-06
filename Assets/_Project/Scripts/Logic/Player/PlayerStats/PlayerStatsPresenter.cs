using System;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using _Project.Scripts.Logic.Player.PlayerStats.UI;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.Sound;
using _Project.Scripts.Services.UpgradePoints;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.PlayerStats
{
    public class PlayerStatsPresenter : IDisposable, ITickable
    {
        private readonly IInputService _inputService;
        private readonly IGamePauseService _pauseService;
        private readonly IUpgradePointsService _upgradePoints;
        private readonly IAudioService _audioService;
        
        private readonly PlayerStatsModel _model;
        private readonly PlayerStatsData _statsData;
        private readonly AudioSource _audioSource;
        private readonly AudioClip _levelUpSound;
        
        private PlayerStatsView _view;
        private PlayerDeath _playerDeath;

        private bool _isOpen;

        public PlayerStatsPresenter(IInputService inputService, IGamePauseService pauseService, IAudioService audioService,
            PlayerStatsModel model, PlayerStatsData statsData, IUpgradePointsService upgradePoints, AudioSource audioSource,
            AudioClip levelUpSound)
        {
            _inputService = inputService;
            _model = model;
            _statsData = statsData;
            _pauseService = pauseService;
            _upgradePoints = upgradePoints;
            _audioService = audioService;
            _audioSource = audioSource;
            _levelUpSound = levelUpSound;
        }
        
        public void Construct(PlayerStatsView view, PlayerDeath playerDeath)
        {
            _view = view;
            _playerDeath = playerDeath;
        }
        
        public async UniTask InitializeAsync()
        {
            _model.OnStatsChanged += UpdateStatItems;
            _view.OnOpenButtonClicked += Open;
            _view.OnCloseButtonClicked += Close;
            _view.OnApplyChangesButtonClicked += ApplyChanges;
            _upgradePoints.OnPointAdded += PlayLevelUpSound;
            
            await _view.CreateStatItemsAsync(_statsData.GetStatValues());
            
            foreach (PlayerStatItemView statItemView in _view.GetStatItems())
                statItemView.OnUpgradeButtonClicked += UpgradeStatItem;
        }

        public void Dispose()
        {
            _model.OnStatsChanged -= UpdateStatItems;
            _view.OnOpenButtonClicked -= Open;
            _view.OnCloseButtonClicked -= Close;
            _view.OnApplyChangesButtonClicked -= ApplyChanges;
            _upgradePoints.OnPointAdded -= PlayLevelUpSound;
            
            foreach (PlayerStatItemView statItemView in _view.GetStatItems())
                statItemView.OnUpgradeButtonClicked -= UpgradeStatItem;
        }

        public void Tick()
        {
            if (_inputService.IsOpenStatsButtonPressed()) 
                Open();
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
            _pauseService.SetPaused(true);
            _view.ShowWindow();
            UpdateStatItems();
        }

        private async void Close()
        {
            _isOpen = false;
            _pauseService.SetPaused(false);
            await _view.HideWindowAsync();
            _model.DiscardPreviewChanges();
        }

        private void ApplyChanges()
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
            foreach (var stat in _statsData.GetStatValues())
                UpdateStatItem(stat.Name);
        }

        private void UpdateStatItem(StatName statName)
        {
            PlayerStatData stat = _statsData.GetStat(statName);
            bool canUpgrade = _model.CanUpgrade(statName);
            _view.UpdateStatItem(statName, stat.PreviewLevel, canUpgrade);
        }
        
        private void PlayLevelUpSound() => 
            _audioService.PlayOneShot(_levelUpSound, _audioSource);
    }
}
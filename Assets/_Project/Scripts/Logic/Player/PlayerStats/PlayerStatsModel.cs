using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using _Project.Scripts.Services.Sound;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Logic.Player.PlayerStats
{
    public class PlayerStatsModel: IDisposable
    {
        public event Action OnStatsChanged;
        
        private readonly PlayerStatsData _statsData;
        private readonly PlayerStatsSaveLoad _saveLoad;
        private readonly IAudioService _audioService;
        
        private readonly AudioSource _audioSource;
        private readonly AudioClip _levelUpSound;

        public int UpgradePoints { get; private set; }
        
        public PlayerStatsModel(PlayerStatsData statsData, PlayerStatsSaveLoad saveLoad, IAudioService audioService,
            AudioSource audioSource, AudioClip levelUpSound)
        {
            _audioService = audioService;
            _statsData = statsData;
            _saveLoad = saveLoad;
            _audioSource = audioSource;
            _levelUpSound = levelUpSound;
        }

        public async UniTask Initialize()
        {
            Dictionary<StatName,PlayerStatData> statsData = await _statsData.CreateStatsAsync();

            foreach (PlayerStatData statData in statsData.Values) 
                statData.OnStatChanged += InvokeStatChanged;

            PlayerStatsProgress progress = await _saveLoad.LoadStats();
            UpgradePoints = progress.UpgradePoints;
        }

        public void Dispose()
        {
            foreach (PlayerStatData stat in _statsData.GetStats())
                stat.OnStatChanged -= InvokeStatChanged;
        }

        public async void ApplyChanges()
        {
            if (!HasAnyChanges()) 
                return;
            
            foreach (PlayerStatData stat in _statsData.GetStats()) 
                stat.ApplyPreviewLevel();
            
            await _saveLoad.SaveStats(UpgradePoints);
        }

        public void DiscardPreviewChanges()
        {
            if (!HasAnyChanges()) 
                return;
            
            int returnedPoints = 0;

            foreach (PlayerStatData stat in _statsData.GetStats())
            {
                returnedPoints += stat.PreviewLevel - stat.Level;
                stat.DiscardPreviewLevel();
            }
               
            UpgradePoints += returnedPoints;
            OnStatsChanged?.Invoke();
        }

        public async UniTask AddUpgradePoint(int points = 1)
        {
            UpgradePoints += points;
            OnStatsChanged?.Invoke();
            _audioService.PlayOneShot(_levelUpSound, _audioSource);
            await _saveLoad.SaveStats(UpgradePoints);
        }

        public void UpgradeStat(StatName statName)
        {
            if (!CanUpgrade(statName))
                return;

            _statsData.GetStat(statName).IncreasePreviewLevel();
            UpgradePoints--;
            OnStatsChanged?.Invoke();
        }
        
        public bool CanUpgrade(StatName statName)
        {
            if (UpgradePoints <=0 || !_statsData.Stats.ContainsKey(statName))
                return false;

            return _statsData.GetStat(statName).PreviewLevel < _statsData.GetStat(statName).MaxLevel;
        }
        
        private bool HasAnyChanges() =>
            _statsData.GetStats().Any(stat => stat.PreviewLevelHasChanged);
        
        private void InvokeStatChanged() => 
            OnStatsChanged?.Invoke();
    }
}
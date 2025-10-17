using System;
using _Project._Scripts.Configs;
using UnityEngine;

namespace _Project._Scripts.StatSystem
{
    public class PlayerStat
    {
        public event Action OnStatChanged;
        
        public StatName Name { get; private set; }
        public float BaseValue { get; private set; }
        public float CurrentValue { get; private set; }
        public float IncrementPerLevel { get; private set; }
        public float MaxMultiplier { get; private set; }
        
        public int Level { get; private set; }
        public int PreviewLevel { get; private set; }
        public int MaxLevel => Mathf.FloorToInt((MaxMultiplier - 1) / IncrementPerLevel);

        public PlayerStat(PlayerStatConfig config)
        {
            Name = config.Name;
            BaseValue = config.BaseValue;
            IncrementPerLevel = config.IncrementPerLevel;
            MaxMultiplier = config.MaxMultiplier;
            Level = 0;
            PreviewLevel = 0;
            RecalculateCurrentValue();
        }

        public void RecalculateCurrentValue()
        {
            CurrentValue = Mathf.Min(
                BaseValue * (1 + IncrementPerLevel * Level), 
                BaseValue * MaxMultiplier);
            OnStatChanged?.Invoke();
        }
        
        public void SetLevel(int level)
        {
            Level = level;
            PreviewLevel = level;
            RecalculateCurrentValue();
        }

        public void ApplyPreviewLevel()
        {
            Level = PreviewLevel;
            RecalculateCurrentValue();
        }

        public void DiscardPreviewLevel()
        {
            PreviewLevel = Level;
            OnStatChanged?.Invoke();
        }

        public void IncreasePreviewLevel()
        {
            PreviewLevel++;
            OnStatChanged?.Invoke();
        }
    }
}
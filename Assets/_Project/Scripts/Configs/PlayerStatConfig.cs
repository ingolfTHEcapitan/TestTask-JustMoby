using System;
using _Project.Scripts.Logic.PlayerStats;
using UnityEngine;

namespace _Project.Scripts.Configs
{
    [Serializable]
    public class PlayerStatConfig
    {
        [field: SerializeField] public StatName Name { get; private set; }
        [field: SerializeField] public float BaseValue { get; private set; }
        [field: SerializeField] public float IncrementPerLevel { get; private set; }
        [field: SerializeField] public float MaxMultiplier { get; private set; }
        [field: SerializeField] public string IconFrameAddress { get; private set; }
        [field: SerializeField] public string IconAddress { get; private set; }
    }
}
using System;
using UnityEngine.Serialization;

namespace _Project._Scripts.Data
{
    [Serializable]
    public class PlayerProgress
    {
        public int UpgradePoints;
        public int HealthLevel;
        public int SpeedLevel;
        public int DamageLevel;
    }
}
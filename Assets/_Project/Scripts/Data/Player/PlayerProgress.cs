using System;
using _Project.Scripts.Data.IAP;

namespace _Project.Scripts.Data.Player
{
    [Serializable]
    public class PlayerProgress
    {
        public PlayerStatsProgress PlayerStatsProgress = new PlayerStatsProgress();
        public PurchaseData PurchaseData = new PurchaseData();
        public AudioSettingsData AudioSettingsData = new AudioSettingsData();
        public long LastSaveTimeUnix;
    }
}
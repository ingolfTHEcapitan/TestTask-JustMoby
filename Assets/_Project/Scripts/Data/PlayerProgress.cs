using System;
using _Project.Scripts.Data.IAP;

namespace _Project.Scripts.Data
{
    [Serializable]
    public class PlayerProgress
    {
        public PlayerStatsData PlayerStatsData = new PlayerStatsData();
        public PurchaseData PurchaseData = new PurchaseData();
    }
}
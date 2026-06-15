using System;
using _Project.Scripts.Data.IAP;

namespace _Project.Scripts.Data.Player
{
    [Serializable]
    public class PlayerProgress
    {
        public PlayerStatsData PlayerStatsData = new PlayerStatsData();
        public PurchaseData PurchaseData = new PurchaseData();
        public long LastSaveTimeUnix;

        public string GetFormatedSaveTime(bool getLocalDateTime = true)
        {
            DateTimeOffset unixSaveTime = DateTimeOffset.FromUnixTimeSeconds(LastSaveTimeUnix);

            if (getLocalDateTime) 
                unixSaveTime = unixSaveTime.LocalDateTime;
            
            return unixSaveTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
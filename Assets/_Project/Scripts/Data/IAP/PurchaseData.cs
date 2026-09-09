using System;
using System.Collections.Generic;

namespace _Project.Scripts.Data.IAP
{
    [Serializable]
    public class PurchaseData
    {
        public bool IsAdsRemoved;
        public List<BoughtIAP> BoughtIAPs = new List<BoughtIAP>();
    }
}
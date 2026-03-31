using System;
using System.Collections.Generic;

namespace _Project.Scripts.Data.IAP
{
    [Serializable]
    public class PurchaseData
    {
        public event Action OnChanged;
        
        public bool IsAdsRemoved;
        public List<BoughtIAP> boughtIAPs = new List<BoughtIAP>();

        public void AddPurchase(string id)
        {
            BoughtIAP boughtIAP = boughtIAPs.Find(x => x.IAPid == id);

            if (boughtIAP != null)
                boughtIAP.Count++;
            else
                boughtIAPs.Add(new BoughtIAP { IAPid = id, Count = 1});
            
            OnChanged?.Invoke();
        }
    }
}
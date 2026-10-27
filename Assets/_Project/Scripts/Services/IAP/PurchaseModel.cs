using System;
using System.Collections.Generic;
using _Project.Scripts.Data.IAP;

namespace _Project.Scripts.Services.IAP
{
    public class PurchaseModel
    {
        public event Action OnPurchaseAdd;
        
        private readonly ProgressService _progress;
        
        private List<BoughtIAP> BoughtIAPs => _progress.PlayerProgress.PurchaseData.BoughtIAPs;

        public PurchaseModel(ProgressService progress) => 
            _progress = progress;

        public void AddPurchase(string id)
        {
            BoughtIAP boughtIAP = BoughtIAPs.Find(x => x.IAPid == id);

            if (boughtIAP != null)
                boughtIAP.Count++;
            else
                BoughtIAPs.Add(new BoughtIAP { IAPid = id, Count = 1});
            
            OnPurchaseAdd?.Invoke();
        }
    }
}
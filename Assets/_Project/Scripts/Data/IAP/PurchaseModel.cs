using System;
using System.Collections.Generic;
using _Project.Scripts.Services.Progress;

namespace _Project.Scripts.Data.IAP
{
    public class PurchaseModel
    {
        public event Action OnChanged;
        
        private readonly IProgressService _progress;
        
        private List<BoughtIAP> BoughtIAPs => _progress.PlayerProgress.PurchaseData.boughtIAPs;

        public PurchaseModel(IProgressService progress) => 
            _progress = progress;

        public void AddPurchase(string id)
        {
            BoughtIAP boughtIAP = BoughtIAPs.Find(x => x.IAPid == id);

            if (boughtIAP != null)
                boughtIAP.Count++;
            else
                BoughtIAPs.Add(new BoughtIAP { IAPid = id, Count = 1});
            
            OnChanged?.Invoke();
        }
    }
}
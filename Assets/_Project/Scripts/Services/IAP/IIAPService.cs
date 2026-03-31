using System;
using System.Collections.Generic;
using _Project.Scripts.Configs.IAP;

namespace _Project.Scripts.Services.IAP
{
    public interface IIAPService
    {
        event Action OnPurchaseInitialized;
        bool IsInitialized { get; }
        void Initialize();
        void StartPurchase(string productId);
        List<ProductDescription> GetProducts();
        void Dispose();
    }
}
using System;
using System.Collections.Generic;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Data.IAP;
using _Project.Scripts.Services.IAP;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopWindowModel: IDisposable
    {
        public event Action OnPurchaseAdd;
        
        private readonly IIAPService _iapService;
        private readonly PurchaseModel _purchaseModel;

        public bool IapServiceIsInitialized => _iapService.IsInitialized;
        
        public ShopWindowModel(IIAPService iapService, PurchaseModel purchaseModel)
        {
            _iapService = iapService;
            _purchaseModel = purchaseModel;
        }

        public void Initialize() => 
            _purchaseModel.OnPurchaseAdd += InvokeOnPurchaseAdd;

        public void Dispose() => 
            _purchaseModel.OnPurchaseAdd -= InvokeOnPurchaseAdd;

        public List<ProductDescription> GetProducts() => 
            _iapService.GetProducts();

        private void InvokeOnPurchaseAdd() =>
            OnPurchaseAdd?.Invoke();
    }
}
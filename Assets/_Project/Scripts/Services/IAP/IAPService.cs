using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Data.IAP;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using UnityEngine.Purchasing;
using ProductDescription = _Project.Scripts.Configs.IAP.ProductDescription;

namespace _Project.Scripts.Services.IAP
{
    public class IAPService : IIAPService, IDisposable
    {
        private readonly PurchaseData _purchaseData;
        public event Action OnPurchaseInitialized;
        
        private readonly IAPProvider _iapProvider;
        private readonly IProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;

        public bool IsInitialized => _iapProvider.IsInitialized;
        public IAPService(IProgressService progressService, ProductConfigWrapper productConfigWrapper,
            ISaveLoadService saveLoadService)
        {
            _iapProvider = new IAPProvider(productConfigWrapper);
            _progressService = progressService;
            _saveLoadService = saveLoadService;
        }

        public void Initialize()
        {
            _iapProvider.OnPurchaseInitialized += InvokeOnPurchaseInitialized;
            _iapProvider.OnProcessPurchase += ProcessPurchase;
            _iapProvider.Initialize();
        }

        public void Dispose()
        {
            _iapProvider.OnPurchaseInitialized -= InvokeOnPurchaseInitialized;
            _iapProvider.OnProcessPurchase -= ProcessPurchase;
        }

        public void StartPurchase(string productId) => 
            _iapProvider.StartPurchase(productId);

        public List<ProductDescription> GetProducts() =>
            GetProductDescriptions().ToList();

        private PurchaseProcessingResult ProcessPurchase(Product purchaseProduct)
        {
            ProductConfig productConfig = _iapProvider.ProductConfigs[purchaseProduct.definition.id];
            
            switch (productConfig.ItemType)
            {
                case ItemType.RemoveAds:
                    _progressService.PlayerProgress.PurchaseData.IsAdsRemoved = true;
                    _progressService.PlayerProgress.PurchaseData.AddPurchase(purchaseProduct.definition.id);
                    break;
                case ItemType.Gems:
                    _progressService.PlayerProgress.PurchaseData.AddPurchase(purchaseProduct.definition.id);
                    break;
            }
            
            _saveLoadService.SaveProgress(_progressService);
            
            return PurchaseProcessingResult.Complete;
        }

        private IEnumerable<ProductDescription> GetProductDescriptions()
        {
            PurchaseData purchaseData = _progressService.PlayerProgress.PurchaseData;

            foreach (string productId in _iapProvider.Products.Keys)
            {
                Product product = _iapProvider.Products[productId];
                ProductConfig productConfig = _iapProvider.ProductConfigs[productId];
                BoughtIAP boughtIAP = purchaseData.boughtIAPs.Find(x => x.IAPid == productId);
                
                if (ProductBoughtOut(boughtIAP, productConfig))
                    continue;

                yield return new ProductDescription
                {
                    Id = productId,
                    Product = product,
                    ProductConfig = productConfig,
                    AvailablePurchasesLeft = boughtIAP != null 
                        ? productConfig.MaxPurchaseCount - boughtIAP.Count 
                        : productConfig.MaxPurchaseCount,
                };
            }
        }

        private bool ProductBoughtOut(BoughtIAP boughtIAP, ProductConfig productConfig) => 
            boughtIAP != null && boughtIAP.Count >= productConfig.MaxPurchaseCount;

        private void InvokeOnPurchaseInitialized() => 
            OnPurchaseInitialized?.Invoke();
    }
}
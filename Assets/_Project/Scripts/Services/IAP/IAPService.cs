using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Data.IAP;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.Services.SaveLoad.CloudSave;
using Cysharp.Threading.Tasks;
using UnityEngine.Purchasing;
using Zenject;
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
        private UniTaskCompletionSource<bool> _purchaseTaskCompletionSource;

        public bool IsInitialized => _iapProvider.IsInitialized;
        public IAPService(IProgressService progressService, ProductConfigWrapper productConfigWrapper,
            [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService)
        {
            _iapProvider = new IAPProvider(productConfigWrapper);
            _progressService = progressService;
            _saveLoadService = saveLoadService;
        }

        public void Initialize()
        {
            _iapProvider.OnPurchaseInitialized += InvokeOnPurchaseInitialized;
            _iapProvider.OnProcessPurchase += ProcessPurchase;
            _iapProvider.OnPurchaseFailedAction += HandlePurchaseFailed;
            _iapProvider.Initialize();
        }

        public void Dispose()
        {
            _iapProvider.OnPurchaseInitialized -= InvokeOnPurchaseInitialized;
            _iapProvider.OnProcessPurchase -= ProcessPurchase;
        }

        public async UniTask<bool> StartPurchase(string productId)
        {
            if (_purchaseTaskCompletionSource != null)
                return false;
            
            _purchaseTaskCompletionSource = new UniTaskCompletionSource<bool>();
            
            _iapProvider.StartPurchase(productId);

            bool result = await _purchaseTaskCompletionSource.Task;
            _purchaseTaskCompletionSource = null;
            return result;
        }

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
            
            _purchaseTaskCompletionSource.TrySetResult(true);
            _saveLoadService.SaveProgressAsync(_progressService);
            
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

        private void HandlePurchaseFailed(string obj) => 
            _purchaseTaskCompletionSource.TrySetResult(false);
    }
}
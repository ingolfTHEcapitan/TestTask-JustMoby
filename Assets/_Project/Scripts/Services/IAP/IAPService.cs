using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Data.IAP;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.Services.SaveLoad;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;
using ProductDescription = _Project.Scripts.Configs.IAP.ProductDescription;

namespace _Project.Scripts.Services.IAP
{
    public class IAPService : IIAPService
    {
        private readonly PurchaseData _purchaseData;
        private readonly IAPProvider _iapProvider;
        private readonly IProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly PurchaseModel _purchaseModel;
        private UniTaskCompletionSource<bool> _purchaseTaskCompletionSource;

        public bool IsInitialized => _iapProvider.IsInitialized;
        public IAPService(IProgressService progressService, IAPProvider iapProvider, PurchaseModel purchaseModel,
            [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService)
        {
            _iapProvider = iapProvider;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _purchaseModel = purchaseModel;
        }

        public void Initialize()
        {
            _iapProvider.OnProcessPurchase += ProcessPurchase;
            _iapProvider.OnPurchaseFailedAction += HandlePurchaseFailed;
            _iapProvider.Initialize();
        }

        public void Dispose()
        {
            _iapProvider.OnProcessPurchase -= ProcessPurchase;
            _iapProvider.OnPurchaseFailedAction -= HandlePurchaseFailed;
        }

        public async UniTask<bool> TryStartPurchaseAsync(ProductDescription productDescription)
        {
            if (_purchaseTaskCompletionSource != null)
                return false;
            
            _purchaseTaskCompletionSource = new UniTaskCompletionSource<bool>();
            
            _iapProvider.StartPurchase(productDescription);

            bool result = await _purchaseTaskCompletionSource.Task;
            _purchaseTaskCompletionSource = null;
            return result;
        }

        public List<ProductDescription> GetProducts() =>
            GetProductDescriptions().ToList();

        public ProductDescription GetProductById(string productId) =>
            BuildProductDescription(productId);

        private PurchaseProcessingResult ProcessPurchase(Product purchaseProduct)
        {
            ProductConfig productConfig = _iapProvider.GetProductConfig(purchaseProduct.definition.id);
            
            switch (productConfig.ItemType)
            {
                case ItemType.RemoveAds:
                    _progressService.PlayerProgress.PurchaseData.IsAdsRemoved = true;
                    _purchaseModel.AddPurchase(purchaseProduct.definition.id);
                    break;
                case ItemType.Gems:
                    _purchaseModel.AddPurchase(purchaseProduct.definition.id);
                    break;
            }

            ProcessPurchaseAsync().Forget();

            return PurchaseProcessingResult.Complete;
        }

        private async UniTaskVoid ProcessPurchaseAsync()
        {
            try
            {
                await _saveLoadService.SaveProgressAsync(_progressService);
                _purchaseTaskCompletionSource.TrySetResult(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"[IAP UNITY] Progress could not be saved during purchase.\nMessage: {e.Message}");
                _purchaseTaskCompletionSource.TrySetResult(false);
            }
        }

        private IEnumerable<ProductDescription> GetProductDescriptions()
        {
            foreach (string productId in _iapProvider.GetProductIds())
            {
                ProductDescription productDescription = BuildProductDescription(productId);
                
                if (productDescription.AvailablePurchasesLeft <= 0)
                    continue;
                
                yield return productDescription;
            }
        }

        private ProductDescription BuildProductDescription(string productId)
        {
            PurchaseData purchaseData = _progressService.PlayerProgress.PurchaseData;
            Product product = _iapProvider.GetProduct(productId);
            ProductConfig productConfig = _iapProvider.GetProductConfig(productId);
            BoughtIAP boughtIAP = purchaseData.BoughtIAPs.Find(x => x.IAPid == productId);
            
            return new ProductDescription
            {
                Id = productId,
                Product = product,
                ProductConfig = productConfig,
                AvailablePurchasesLeft = boughtIAP != null 
                    ? productConfig.MaxPurchaseCount - boughtIAP.Count 
                    : productConfig.MaxPurchaseCount,
            };
        }

        private void HandlePurchaseFailed(string obj) => 
            _purchaseTaskCompletionSource.TrySetResult(false);
    }
}
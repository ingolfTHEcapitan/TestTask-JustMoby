using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Configs.IAP;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using ProductDescription = _Project.Scripts.Configs.IAP.ProductDescription;

namespace _Project.Scripts.Services.IAP
{
    public class IAPProvider: IDetailedStoreListener
    {
        public event Action<string> OnPurchaseFailedAction;
        public event Func<Product, PurchaseProcessingResult> OnProcessPurchase;

        private Dictionary<string, ProductConfig> _productConfigs;
        private readonly Dictionary<string, Product> _products = new Dictionary<string, Product>();

        private IStoreController _controller;
        private IExtensionProvider _extensions;
        private readonly ProductConfigWrapper _productConfigWrapper;

        public bool IsInitialized => _controller != null && _extensions != null;
        
        public IAPProvider(ProductConfigWrapper productConfigWrapper) => 
            _productConfigWrapper = productConfigWrapper;

        public void Initialize()
        {
            _productConfigs = _productConfigWrapper.Configs.ToDictionary(x => x.Id, x => x);
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            
            AddProducts(_productConfigs, builder);
            
            UnityPurchasing.Initialize(this, builder);
        }

        public void StartPurchase(ProductDescription productDescription) => 
            _controller.InitiatePurchase(productDescription.Id);

        public ProductConfig GetProductConfig(string productId) =>
            _productConfigs[productId];
        
        public Product GetProduct(string productId) => 
            _products[productId];
        
        public IEnumerable<string> GetProductIds() => 
            _products.Keys;

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _extensions = extensions;
            _controller = controller;
            
            foreach (var product in controller.products.all)
                _products.Add(product.definition.id, product);
            
            Debug.Log("UnityPurchasing initialization success");
        }
        
        public void OnInitializeFailed(InitializationFailureReason error) => 
            Debug.LogError($"UnityPurchasing OnInitializeFailed: {error}");

        public void OnInitializeFailed(InitializationFailureReason error, string message) => 
            Debug.LogError($"UnityPurchasing OnInitializeFailed: {error}, message: {message}");

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log($"UnityPurchasing ProcessPurchase success: {purchaseEvent.purchasedProduct.definition.id}");
            
            return OnProcessPurchase.Invoke(purchaseEvent.purchasedProduct);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"product {product.definition.id} purchase failed, Purchase Failure Reason: {failureReason}," +
                           $" transaction id: {product.transactionID}");
            OnPurchaseFailedAction?.Invoke(product.definition.id);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError($"product {product.definition.id} purchase failed, Purchase Failure Description: {failureDescription.message}," +
                           $" transaction id: {product.transactionID}");
            OnPurchaseFailedAction?.Invoke(product.definition.id);
        }
        
        private void AddProducts(Dictionary<string, ProductConfig> productConfigs, ConfigurationBuilder builder)
        {
            foreach (ProductConfig product in productConfigs.Values)
                builder.AddProduct(product.Id, product.ProductType);
        }
    }
}
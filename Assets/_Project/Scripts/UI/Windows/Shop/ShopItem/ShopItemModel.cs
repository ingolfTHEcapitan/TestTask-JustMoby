using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Services.IAP;
using UnityEngine.Purchasing;

namespace _Project.Scripts.UI.Windows.Shop.ShopItem
{
    public class ShopItemModel
    {
        private readonly IIAPService _iapService;
        private readonly ProductDescription _productDescription;

        public string IconAddress => _productDescription.ProductConfig.IconAddress;
        public string ProductName => _productDescription.ProductConfig.ProductName;
        public string Price => _productDescription.ProductConfig.Price;
        public int Quantity => _productDescription.ProductConfig.Quantity;
        public int PurchasesLeft => _productDescription.AvailablePurchasesLeft;

        public ShopItemModel(IIAPService iapService, ProductDescription productDescription)
        {
            _iapService = iapService;
            _productDescription = productDescription;
        }

        public void StartPurchase() => 
            _iapService.StartPurchaseAsync(_productDescription);

        public bool IsConsumableProductType() => 
            _productDescription.ProductConfig.ProductType == ProductType.Consumable;
    }
}
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Services.IAP;
using Cysharp.Threading.Tasks;
using UnityEngine.Purchasing;

namespace _Project.Scripts.UI.Windows.Shop.ShopItem
{
    public class ShopItemModel
    {
        private readonly IIAPService _iapService;
        private readonly string _productId;

        public string IconAddress => ProductConfig.IconAddress;
        public string ProductName => ProductConfig.ProductName;
        public string Price => ProductConfig.Price;
        public int Quantity => ProductConfig.Quantity;
        public int PurchasesLeft => ProductDescription.AvailablePurchasesLeft;
        public bool CanBuy => PurchasesLeft > 0;
        private ProductDescription ProductDescription => _iapService.GetProductById(_productId);
        private ProductConfig ProductConfig => ProductDescription.ProductConfig;

        public ShopItemModel(IIAPService iapService, ProductDescription productDescription)
        {
            _iapService = iapService;
            _productId = productDescription.Id;
        }

        public async UniTask<bool> TryStartPurchaseAsync() => 
            await _iapService.TryStartPurchaseAsync(ProductDescription);

        public bool IsConsumableProductType() => 
            ProductConfig.ProductType == ProductType.Consumable;
    }
}
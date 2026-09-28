using System.Collections.Generic;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Data.IAP;
using _Project.Scripts.Services.IAP;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopWindowModel
    {
        private readonly IIAPService _iapService;

        public bool IapServiceIsInitialized => _iapService.IsInitialized;
        
        public ShopWindowModel(IIAPService iapService) => 
            _iapService = iapService;

        public List<ProductDescription> GetProducts() => 
            _iapService.GetProducts();
    }
}
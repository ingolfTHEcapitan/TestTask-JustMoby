using UnityEngine.Purchasing;

namespace _Project.Scripts.Configs.IAP
{
    public class ProductDescription
    {
        public string Id;
        public Product Product;
        public ProductConfig ProductConfig;
        public int AvailablePurchasesLeft;
    }
}
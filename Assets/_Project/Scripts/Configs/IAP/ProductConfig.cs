using System;
using UnityEngine.Purchasing;

namespace _Project.Scripts.Configs.IAP
{
    [Serializable]
    public class ProductConfig
    {
        public string Id;
        public string ProductName;
        public ProductType ProductType;
        public int MaxPurchaseCount;
        public ItemType ItemType;
        public int Quantity;
        public string Price;
        public string IconAddress;
    }
}
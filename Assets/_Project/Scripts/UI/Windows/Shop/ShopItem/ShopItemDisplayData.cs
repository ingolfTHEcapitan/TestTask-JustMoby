namespace _Project.Scripts.UI.Windows.Shop.ShopItem
{
    public class ShopItemDisplayData
    {
        public readonly string ProductName;
        public readonly string Price;
        public readonly int Quantity;
        public readonly int PurchasesLeft;
        public readonly bool CanBuy;
        public readonly bool IsConsumable;

        public ShopItemDisplayData(string productName, string price, int quantity, int purchasesLeft, bool canBuy, bool isConsumable)
        {
            ProductName = productName;
            Price = price;
            Quantity = quantity;
            PurchasesLeft = purchasesLeft;
            CanBuy = canBuy;
            IsConsumable = isConsumable;
        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Windows.Shop.ShopItem
{
    public class ShopItemPresenter : IDisposable
    {
        private readonly ShopItemModel _itemModel;
        private readonly ShopItemView _itemView;

        private bool _isPurchaseNow;
        
        public ShopItemPresenter(ShopItemView itemView, ShopItemModel itemModel)
        {
            _itemView = itemView;
            _itemModel = itemModel;
        }

        public void Initialize()
        {
            _itemView.OnBuyButtonClicked += StartPurchase;

            ShopItemDisplayData itemDisplayData = new ShopItemDisplayData(
                _itemModel.ProductName,
                _itemModel.Price,
                _itemModel.Quantity,
                _itemModel.PurchasesLeft,
                _itemModel.CanBuy,
                _itemModel.IsConsumable
            );
            
            _itemView.UpdateItemData(itemDisplayData);
        }

        public void Dispose() => 
            _itemView.OnBuyButtonClicked -= StartPurchase;

        private void StartPurchase()
        {
            if (_isPurchaseNow)
                return;

            StartPurchaseAsync().Forget();
        }

        private async UniTask StartPurchaseAsync()
        {
            try
            {
                _isPurchaseNow = true;
                
                bool success = await _itemModel.TryStartPurchaseAsync();
                _itemView.HandlePurchaseResults(_itemModel.PurchasesLeft, _itemModel.CanBuy, success);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SHOP WINDOW] Purchase failed, message: {e.Message}");
                Debug.LogException(e);
            }
            finally
            {
                _isPurchaseNow = false;
            }
        }
    }
}
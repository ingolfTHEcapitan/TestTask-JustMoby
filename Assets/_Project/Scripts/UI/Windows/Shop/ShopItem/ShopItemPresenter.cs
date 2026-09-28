using System;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Windows.Shop.ShopItem
{
    public class ShopItemPresenter: IDisposable
    {
        private readonly IUIFactory _uiFactory;
        private readonly ShopItemModel _itemModel;
        private readonly ShopItemView _itemView;

        private bool _isPurchased;
        
        public ShopItemPresenter(IUIFactory uiFactory, ShopItemView itemView, ShopItemModel itemModel)
        {
            _uiFactory = uiFactory;
            _itemView = itemView;
            _itemModel = itemModel;
        }

        public async UniTask<ShopItemView> Initialize(AudioSource audioSource)
        {
            _itemView.OnBuyButtonClicked += StartPurchaseAsync;
            _itemView.Initialize(audioSource);

            await FillShopItemAsync();
            return _itemView;
        }

        public void Dispose() => 
            _itemView.OnBuyButtonClicked -= StartPurchaseAsync;

        private async UniTask FillShopItemAsync()
        {
            Sprite icon = await _uiFactory.LoadSpriteAsync(_itemModel.IconAddress);
            _itemView.UpdateItemData(icon, _itemModel.ProductName, _itemModel.Price, _itemModel.PurchasesLeft);
            ToggleQuantityTextVisibility();
        }

        private async void StartPurchaseAsync()
        {
            if (_isPurchased)
                return;
            
            _isPurchased = true;
            _itemView.SetBuyButtonInteractable(false);
            
            try
            {
                bool success = await _itemModel.TryStartPurchaseAsync();

                if (success)
                    _itemView.UpdateAvailablePurchasesLeft(_itemModel.PurchasesLeft);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SHOP WINDOW] Purchase failed, message: {e.Message}");
                Debug.LogException(e);
            }
            finally
            {
                _isPurchased = false;
                RefreshBuyButtonState();
            }
        }

        private void ToggleQuantityTextVisibility()
        {
            if (_itemModel.IsConsumableProductType()) 
                _itemView.UpdateQuantityText(_itemModel.Quantity);
            else
                _itemView.HideQuantityText();
        }

        private void RefreshBuyButtonState()
        {
            bool canBuy = _itemModel.CanBuy;
            _itemView.SetBuyButtonInteractable(canBuy);
            
            if (canBuy)
                _itemView.ShowBuyButton();
            else
                _itemView.HideBuyButton();
        }
    }
}
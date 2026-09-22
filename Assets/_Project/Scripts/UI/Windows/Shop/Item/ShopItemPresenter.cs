using System;
using System.Threading.Tasks;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.UI.Factory;
using _Project.Scripts.UI.Windows.PlayerStats;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI.Windows.Shop.Item
{
    public class ShopItemPresenter: IDisposable
    {
        private readonly IUIFactory _uiFactory;
        private readonly ShopItemModel _model;
        private readonly ShopItemView _view;

        public ShopItemPresenter(IUIFactory uiFactory, ShopItemView view, ShopItemModel model)
        {
            _uiFactory = uiFactory;
            _view = view;
            _model = model;
        }

        public async UniTask<ShopItemView> Initialize(AudioSource audioSource)
        {
            _view.OnBuyButtonClicked += StartPurchase;
            _view.Initialize(audioSource);

            await FillShopItemAsync();
            return _view;
        }

        public void Dispose() => 
            _view.OnBuyButtonClicked -= StartPurchase;

        private async UniTask FillShopItemAsync()
        {
            Sprite icon = await _uiFactory.LoadSpriteAsync(_model.IconAddress);
            _view.UpdateItemData(icon, _model.ProductName, _model.Price, _model.PurchasesLeft);
            ToggleQuantityTextVisibility();
        }

        private void StartPurchase() => 
            _model.StartPurchase();

        private void ToggleQuantityTextVisibility()
        {
            if (_model.IsConsumableProductType()) 
                _view.UpdateQuantityText(_model.Quantity);
            else
                _view.HideQuantityText();
        }
    }
}
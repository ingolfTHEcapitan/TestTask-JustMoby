using System;
using System.Collections.Generic;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.UI.Windows.Shop.ShopItem;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopWindowPresenter: IDisposable
    {
        private readonly ShopWindowView _view;
        private readonly ShopWindowModel _model;
        private readonly ShopItemFactory _shopItemFactory;
        
        private readonly List<ShopItemView> _shopItemViews = new List<ShopItemView>();
        
        public bool IapServiceIsInitialized => _model.IapServiceIsInitialized;
        
        public ShopWindowPresenter(ShopWindowView view, ShopWindowModel model, ShopItemFactory shopItemFactory)
        {
            _view = view;
            _model = model;
            _shopItemFactory = shopItemFactory;
        }

        public async UniTask InitializeAsync()
        {
            _view.Initialize();
            _view.OnCloseButtonClicked += Close;
            _view.ClearProductsContainer();
            await RefreshAvailableShopItemsAsync();
        }

        public async UniTask OpenAsync()
        {
            await RefreshAvailableShopItemsAsync();
            await _view.OpenAsync();
        }
        
        public void Dispose()
        {
            _view.OnCloseButtonClicked -= Close;
            _shopItemFactory.Dispose();
            ClearShopItems();
        }

        private void Close() => 
            _view.CloseAsync().Forget();

        private async UniTask RefreshAvailableShopItemsAsync()
        {
            ClearShopItems();
            _view.UpdateShopUnavailableObjects(IapServiceIsInitialized);

            if (IapServiceIsInitialized) 
                await FillShopItemsAsync();
        }
        
        private async UniTask FillShopItemsAsync()
        {
            foreach (ProductDescription productDescription in _model.GetProducts())
            {
                ShopItemView shopItemView = 
                    await _shopItemFactory.CreateShopItemAsync(_view.ProductsContainer, productDescription, _view.AudioSource);
                _shopItemViews.Add(shopItemView);
            }
        }
        
        private void ClearShopItems()
        {
            foreach (ShopItemView shopItemView in _shopItemViews)
                if (shopItemView)
                    Object.Destroy(shopItemView.gameObject);
            
            _shopItemViews.Clear();
        }
    }
}
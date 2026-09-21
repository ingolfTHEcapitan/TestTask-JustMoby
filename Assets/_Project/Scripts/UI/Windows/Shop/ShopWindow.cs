using System.Collections.Generic;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Data.IAP;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using _Project.Scripts.UI.Windows.Shop.Item;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopWindow: MonoBehaviour, IWindow
    {
        // V
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _windowContent;
        [SerializeField] private GameObject[] _shopUnavailableObjects;
        [SerializeField] private Transform _productsContainer;
        [SerializeField] private Button _closeButton;
        // V
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        
        // M
        private IIAPService _iapService;
        // P
        private readonly List<ShopItemView> _shopItemViews = new List<ShopItemView>();
        // M
        private PurchaseModel _purchaseModel;
        private ShopItemUIFactory _shopItemUIFactory;

        [Inject]
        private void Construct(IIAPService iapService, PurchaseModel purchaseModel)
        {
            // M
            _iapService = iapService;
            // M
            _purchaseModel = purchaseModel;
        }

        public void Initialize(ShopItemUIFactory shopItemUIFactory)
        {
            _shopItemUIFactory = shopItemUIFactory;
            // V
            _windowContent.SetActive(false);
            _closeButton.onClick.AddListener(Close);
            // M
            _purchaseModel.OnChanged += RefreshAvailableShopItems;
            
            // V
            ClearProductsContainer();
            // MVP
            RefreshAvailableShopItems();
        }

        // MV
        private void OnDestroy()
        {
            // V
            _closeButton.onClick.RemoveListener(Close);
            // M
            _purchaseModel.OnChanged -= RefreshAvailableShopItems;
            _shopItemUIFactory.Dispose();
        }

        // V
        public void Open()
        {
            // MVP
            RefreshAvailableShopItems();
            _windowContent.SetActive(true);
            _windowAnimation.AnimateOpen();
        }

        // V
        private async void Close()
        {
            await _windowAnimation.AnimateCloseAsync();
            _windowContent.SetActive(false);
        }

        // MVP
        private async void RefreshAvailableShopItems()
        {
            // V
            UpdateShopUnavailableObjects(!_iapService.IsInitialized);
            
            // M
            if (!_iapService.IsInitialized)
                return;
            
            // P
            ClearShopItems();
            await FillShopItemsAsync();
        }

        // V
        private void ClearProductsContainer()
        {
            foreach (Transform child in _productsContainer) 
                Destroy(child.gameObject);
        }

        // P
        private void ClearShopItems()
        {
            foreach (ShopItemView shopItemView in _shopItemViews)
                if (shopItemView)
                    Destroy(shopItemView.gameObject);
        }

        // P
        private async UniTask FillShopItemsAsync()
        {
            // P
            foreach (ProductDescription productDescription in _iapService.GetProducts())
            {
                // P+
                //ShopItem shopItem = await _uiFactory.CreateShopItemAsync(_productsContainer);
                // P
                ShopItemView shopItemView = await _shopItemUIFactory.CreateShopItemAsync(_productsContainer, productDescription, _audioSource);
                _shopItemViews.Add(shopItemView);
                
                // V+
                //await shopItem.InitializeAsync(productDescription, _audioSource);
            }
        }

        // V
        private void UpdateShopUnavailableObjects(bool iapServiceIsInitialized)
        {
            foreach (GameObject shopUnavailableObject in _shopUnavailableObjects) 
                shopUnavailableObject.SetActive(iapServiceIsInitialized);
        }
    }
}
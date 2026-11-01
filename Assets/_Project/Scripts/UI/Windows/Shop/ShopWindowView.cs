using System;
using System.Collections.Generic;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Windows.Shop.ShopItem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopWindowView : MonoBehaviour, IWindow
    {
        public event Action OnWindowDestroy;
        public event Action OnCloseButtonClicked;
        
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _windowContent;
        [SerializeField] private GameObject[] _shopUnavailableObjects;
        [SerializeField] private Button _closeButton;
        [SerializeField] public RectTransform _productsContainer;
        [Header("Audio")]
        [SerializeField,] public AudioSource _audioSource;

        private ShopItemFactory _shopItemFactory;
        private readonly List<ShopItemView> _shopItemViews = new List<ShopItemView>();

        [Inject]
        public void Construct(ShopItemFactory shopItemFactory) => 
            _shopItemFactory = shopItemFactory;

        public void Initialize()
        {
            _windowContent.SetActive(false);
            _closeButton.onClick.AddListener(InvokeOnCloseButtonClicked);
        }

        private void OnDestroy()
        {
            OnWindowDestroy?.Invoke();
            _closeButton.onClick.RemoveListener(InvokeOnCloseButtonClicked);
            ClearShopItems();
        }

        public async UniTask OpenAsync()
        {
            _windowContent.SetActive(true);
            await _windowAnimation.AnimateOpenAsync();
        }
        
        public async UniTask CloseAsync()
        {
            await _windowAnimation.AnimateCloseAsync();
            _windowContent.SetActive(false);
        }

        public async UniTask RefreshAvailableItemsAsync(bool iapIsInitialized, List<ProductDescription> products)
        {
            ClearShopItems();
            UpdateShopUnavailableObjects(iapIsInitialized);

            if (iapIsInitialized) 
                await FillShopItemsAsync(products);
        }

        private void ClearShopItems()
        {
            foreach (Transform shopItem in _productsContainer)
                Destroy(shopItem.gameObject);
            
            _shopItemFactory.Dispose();
            
            foreach (ShopItemView shopItemView in _shopItemViews)
                if (shopItemView)
                    Destroy(shopItemView.gameObject);
            
            _shopItemViews.Clear();
        }

        private async UniTask FillShopItemsAsync(List<ProductDescription> products)
        {
            foreach (ProductDescription productDescription in products)
            {
                ShopItemView shopItem = await _shopItemFactory.CreateAsync(_productsContainer, productDescription, _audioSource);
                _shopItemViews.Add(shopItem);
            }
        }

        private void UpdateShopUnavailableObjects(bool iapServiceIsInitialized)
        {
            foreach (GameObject shopUnavailableObject in _shopUnavailableObjects) 
                shopUnavailableObject.SetActive(!iapServiceIsInitialized);
        }

        private void InvokeOnCloseButtonClicked() => 
            OnCloseButtonClicked?.Invoke();
    }
}
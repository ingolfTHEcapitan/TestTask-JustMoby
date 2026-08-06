using System.Collections.Generic;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Data.IAP;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopWindow: MonoBehaviour, IWindow
    {
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [SerializeField] private GameObject[] _shopUnavailableObjects;
        [SerializeField] private Transform _productsContainer;
        [SerializeField] private Button _closeButton;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        
        private IIAPService _iapService;
        private readonly List<GameObject> _shopItemObjects = new List<GameObject>();
        private IUIFactory _uiFactory;
        private PurchaseModel _purchaseModel;

        [Inject]
        private void Construct(IIAPService iapService, IUIFactory uiFactory, PurchaseModel purchaseModel)
        {
            _iapService = iapService;
            _uiFactory = uiFactory;
            _purchaseModel = purchaseModel;
        }

        public void Initialize()
        {
            _closeButton.onClick.AddListener(Close);
            _purchaseModel.OnChanged += RefreshAvailableShopItems;
            
            RefreshAvailableShopItems();
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(Close);
            _purchaseModel.OnChanged -= RefreshAvailableShopItems;
        }

        public void Open()
        {
            RefreshAvailableShopItems();
            gameObject.SetActive(true);
            _windowAnimation.AnimateOpen();
        }

        private async void Close()
        {
            await _windowAnimation.AnimateCloseAsync();
            gameObject.SetActive(false);
        }

        private async void RefreshAvailableShopItems()
        {
            UpdateShopUnavailableObjects();
            
            if (!_iapService.IsInitialized)
                return;

            ClearShopItems();
            await FillShopItemsAsync();
        }

        private void ClearShopItems()
        {
            foreach (GameObject shopItemObject in _shopItemObjects) 
                Destroy(shopItemObject);
        }

        private async UniTask FillShopItemsAsync()
        {
            foreach (ProductDescription productDescription in _iapService.GetProducts())
            {
                ShopItem shopItem = await _uiFactory.CreateShopItemAsync(_productsContainer);
                _shopItemObjects.Add(shopItem.gameObject);
                await shopItem.InitializeAsync(productDescription, _audioSource);
            }
        }

        private void UpdateShopUnavailableObjects()
        {
            foreach (GameObject shopUnavailableObject in _shopUnavailableObjects) 
                shopUnavailableObject.SetActive(!_iapService.IsInitialized);
        }
    }
}
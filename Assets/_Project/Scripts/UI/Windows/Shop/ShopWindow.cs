using System.Collections.Generic;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.Services.Progress;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopWindow: MonoBehaviour
    {
        [SerializeField] private GameObject[] ShopUnavailableObjects;
        [SerializeField] private Transform _productsContainer;
        [SerializeField] private Button _closeButton;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        
        private IIAPService _iapService;
        private IProgressService _progressService;
        private readonly List<GameObject> _shopItemObjects = new List<GameObject>();
        private IUIFactory _uiFactory;

        [Inject]
        private void Construct(IIAPService iapService, IUIFactory uiFactory, IProgressService progressService)
        {
            _iapService = iapService;
            _uiFactory = uiFactory;
            _progressService = progressService;
        }

        public void Initialize()
        {
            _closeButton.onClick.AddListener(Close);
            _progressService.PlayerProgress.PurchaseData.OnChanged += RefreshAvailableShopItems;
            
            RefreshAvailableShopItems();
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(Close);
            _progressService.PlayerProgress.PurchaseData.OnChanged -= RefreshAvailableShopItems;
        }

        public void Open()
        {
            RefreshAvailableShopItems();
            gameObject.SetActive(true);
        }

        private void Close() => 
            gameObject.SetActive(false);

        private async void RefreshAvailableShopItems()
        {
            UpdateShopUnavailableObjects();
            
            if (!_iapService.IsInitialized)
                return;

            ClearShopItems();
            await FillShopItems();
        }

        private void ClearShopItems()
        {
            foreach (GameObject shopItemObject in _shopItemObjects) 
                Destroy(shopItemObject);
        }

        private async UniTask FillShopItems()
        {
            foreach (ProductDescription productDescription in _iapService.GetProducts())
            {
                ShopItem shopItem = await _uiFactory.CreateShopItem(_productsContainer);
                _shopItemObjects.Add(shopItem.gameObject);
                await shopItem.Initialize(productDescription, _audioSource);
            }
        }

        private void UpdateShopUnavailableObjects()
        {
            foreach (GameObject shopUnavailableObject in ShopUnavailableObjects) 
                shopUnavailableObject.SetActive(!_iapService.IsInitialized);
        }
    }
}
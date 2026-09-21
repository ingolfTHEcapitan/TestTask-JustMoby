using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopItem: MonoBehaviour
    {
        // V+
        [SerializeField] private TextMeshProUGUI _productNameText;
        [SerializeField] private TextMeshProUGUI _quantityText;
        [SerializeField] private TextMeshProUGUI _availablePurchasesLeftText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TextMeshProUGUI _buyButtonPriceText;
        
        // M+
        private IIAPService _iapService;
        // P+
        private IUIFactory _uiFactory;
        //M+
        private ProductDescription _productDescription;

        [Inject]
        private void Construct(IIAPService iapService, IUIFactory uiFactory)
        {
            // P
            _uiFactory = uiFactory;
            // M+
            _iapService = iapService;
        }

        public async UniTask InitializeAsync(ProductDescription productDescription, AudioSource audioSource)
        {
            // M+
            _productDescription = productDescription;
            
            //V+
            _buyButton.onClick.AddListener(StartPurchase);
            ButtonSoundEffect buttonSoundEffect = _buyButton.GetComponent<ButtonSoundEffect>();
            buttonSoundEffect.Initialize(audioSource);
            // P+
            await FillShopItemAsync();
        }

        // V+
        private void OnDestroy() => 
            _buyButton.onClick.RemoveListener(StartPurchase);

        // P+
        private async UniTask FillShopItemAsync()
        {
            // V+ // P+ // M+
            _icon.sprite = await _uiFactory.LoadSpriteAsync(_productDescription.ProductConfig.IconAddress);
            _productNameText.text = _productDescription.ProductConfig.ProductName;
            _buyButtonPriceText.text = _productDescription.ProductConfig.Price;
            _availablePurchasesLeftText.text = _productDescription.AvailablePurchasesLeft.ToString();
            //P+ // V+ // M+
            SetQuantityText();
        }

        // M+
        private void StartPurchase() => 
            _iapService.StartPurchaseAsync(_productDescription);

        //P+ // V+ // M+
        private void SetQuantityText()
        {
            // M
            if (_productDescription.ProductConfig.ProductType == ProductType.Consumable) 
            // V+
                _quantityText.text = _productDescription.ProductConfig.Quantity.ToString();
            else
                // V+
                _quantityText.gameObject.SetActive(false);
        }
    }
}
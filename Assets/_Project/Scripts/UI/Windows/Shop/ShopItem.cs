using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.IAP;
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
        [SerializeField] private TextMeshProUGUI _productNameText;
        [SerializeField] private TextMeshProUGUI _quantityText;
        [SerializeField] private TextMeshProUGUI _availablePurchasesLeftText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TextMeshProUGUI _buyButtonPriceText;
        
        private IIAPService _iapService;
        private IUIFactory _uiFactory;
        private ProductDescription _productDescription;

        [Inject]
        private void Construct(IIAPService iapService, IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            _iapService = iapService;
        }

        public async UniTask Initialize(ProductDescription productDescription, AudioSource audioSource)
        {
            _productDescription = productDescription;
            
            _buyButton.onClick.AddListener(StartPurchase);
            
            ButtonSoundEffect buttonSoundEffect = _buyButton.GetComponent<ButtonSoundEffect>();
            buttonSoundEffect.Initialize(audioSource);
            
            await FillShopItem();
        }

        private void OnDestroy() => 
            _buyButton.onClick.RemoveListener(StartPurchase);

        private async UniTask FillShopItem()
        {
            _productNameText.text = _productDescription.ProductConfig.ProductName;
            _icon.sprite = await _uiFactory.LoadSprite(_productDescription.ProductConfig.IconAddress);
            _buyButtonPriceText.text = _productDescription.ProductConfig.Price;
            _availablePurchasesLeftText.text = _productDescription.AvailablePurchasesLeft.ToString();
            SetQuantityText();
        }

        private void StartPurchase() => 
            _iapService.StartPurchase(_productDescription);

        private void SetQuantityText()
        {
            if (_productDescription.ProductConfig.ProductType == ProductType.Consumable) 
                _quantityText.text = _productDescription.ProductConfig.Quantity.ToString();
            else
                _quantityText.gameObject.SetActive(false);
        }
    }
}
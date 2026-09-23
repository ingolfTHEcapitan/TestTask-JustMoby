using System;
using _Project.Scripts.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.Shop.ShopItem
{
    public class ShopItemView: MonoBehaviour
    {
        public event Action OnBuyButtonClicked;
    
        [SerializeField] private TextMeshProUGUI _productNameText;
        [SerializeField] private TextMeshProUGUI _quantityText;
        [SerializeField] private TextMeshProUGUI _availablePurchasesLeftText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TextMeshProUGUI _buyButtonPriceText;

        public void Initialize(AudioSource audioSource)
        {
            _buyButton.onClick.AddListener(InvokeOnBuyButtonClicked);
            ButtonSoundEffect buttonSoundEffect = _buyButton.GetComponent<ButtonSoundEffect>();
            buttonSoundEffect.Initialize(audioSource);
        }

        private void OnDestroy() => 
            _buyButton.onClick.RemoveListener(InvokeOnBuyButtonClicked);


        public void UpdateItemData(Sprite icon, string productName, string price, int purchasesLeft)
        {
            _icon.sprite = icon;
            _productNameText.text = productName;
            _buyButtonPriceText.text = price;
            _availablePurchasesLeftText.text = purchasesLeft.ToString();
        }

        public void UpdateQuantityText(int quantity) => 
            _quantityText.text = quantity.ToString();

        public void HideQuantityText() => 
            _quantityText.gameObject.SetActive(false);

        private void InvokeOnBuyButtonClicked() => 
            OnBuyButtonClicked?.Invoke();
        
    }
}
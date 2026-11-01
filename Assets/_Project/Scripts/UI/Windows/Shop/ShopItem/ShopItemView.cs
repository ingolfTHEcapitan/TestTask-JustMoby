using System;
using _Project.Scripts.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.Shop.ShopItem
{
    public class ShopItemView : MonoBehaviour
    {
        public event Action OnBuyButtonClicked;
    
        [SerializeField] private TextMeshProUGUI _productNameText;
        [SerializeField] private TextMeshProUGUI _quantityText;
        [SerializeField] private TextMeshProUGUI _availablePurchasesLeftText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TextMeshProUGUI _buyButtonPriceText;

        private AudioSource _audioSource;
        private Sprite _iconSprite;
        
        public void Initialize(AudioSource audioSource, Sprite iconSprite)
        {
            _iconSprite = iconSprite;
            _audioSource = audioSource;
            
            _buyButton.onClick.AddListener(InvokeOnBuyButtonClicked);
            
            if (_buyButton.TryGetComponent(out ButtonSoundEffect soundEffect)) 
                soundEffect.Initialize(_audioSource);
            else
                Debug.LogWarning($"[ShopItemView] No ButtonSoundEffect component attached to {_buyButton.name}");
        }

        private void OnDestroy() => 
            _buyButton.onClick.RemoveListener(InvokeOnBuyButtonClicked);

        public void UpdateItemData(ShopItemDisplayData data)
        {
            _icon.sprite = _iconSprite;
            _productNameText.text = data.ProductName;
            _buyButtonPriceText.text = data.Price;
            UpdateAvailablePurchasesLeft(data.PurchasesLeft);
            ToggleQuantityTextVisibility(data.IsConsumable, data.Quantity);
            
            RefreshBuyButtonState(data.CanBuy);
        }

        public void HandlePurchaseResults(int purchasesLeft, bool canBuy, bool success)
        {
            if (success)
                UpdateAvailablePurchasesLeft(purchasesLeft);
            
            RefreshBuyButtonState(canBuy);
        }
        
        private void ToggleQuantityTextVisibility(bool isConsumable, int quantity)
        {
            if (isConsumable) 
                _quantityText.text = quantity.ToString();
            else
                _quantityText.gameObject.SetActive(false);
        }

        private void InvokeOnBuyButtonClicked()
        {
            _buyButton.interactable = false;
            OnBuyButtonClicked?.Invoke();
        }
        
        private void RefreshBuyButtonState(bool canBuy)
        {
            _buyButton.interactable = canBuy;
            _buyButton.gameObject.SetActive(canBuy);
        }

        private void UpdateAvailablePurchasesLeft(int purchasesLeft) => 
            _availablePurchasesLeftText.text = purchasesLeft.ToString();
    }
}
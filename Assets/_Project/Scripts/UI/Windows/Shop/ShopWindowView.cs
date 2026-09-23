using System;
using _Project.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopWindowView: MonoBehaviour, IWindow
    {
        public event Action OnCloseButtonClicked;
        
        [SerializeField] private WindowPopupAnimation _windowAnimation;
        [Space]
        [SerializeField] private GameObject _windowContent;
        [SerializeField] private GameObject[] _shopUnavailableObjects;
        [SerializeField] private Button _closeButton;
        
        [field: SerializeField] public RectTransform ProductsContainer { get; private set; }
        [field: Header("Audio")]
        [field: SerializeField] public AudioSource AudioSource { get; private set;}

        public void Initialize()
        {
            _windowContent.SetActive(false);
            _closeButton.onClick.AddListener(InvokeOnCloseButtonClicked);
        }

        private void OnDestroy() => 
            _closeButton.onClick.RemoveListener(InvokeOnCloseButtonClicked);
        
        public void Open()
        {
            _windowContent.SetActive(true);
            _windowAnimation.AnimateOpen();
        }
        
        public async void Close()
        {
            await _windowAnimation.AnimateCloseAsync();
            _windowContent.SetActive(false);
        }
        
        public void ClearProductsContainer()
        {
            foreach (RectTransform child in ProductsContainer) 
                Destroy(child.gameObject);
        }
        
        public void UpdateShopUnavailableObjects(bool iapServiceIsInitialized)
        {
            foreach (GameObject shopUnavailableObject in _shopUnavailableObjects) 
                shopUnavailableObject.SetActive(!iapServiceIsInitialized);
        }

        private void InvokeOnCloseButtonClicked() => 
            OnCloseButtonClicked?.Invoke();
    }
}
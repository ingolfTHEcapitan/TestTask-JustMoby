using System;
using _Project.Scripts.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopIWindowView: MonoBehaviour, IWindow
    {
        public event Action OnWindowDestroy;
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

        private void OnDestroy()
        {
            OnWindowDestroy?.Invoke();
            _closeButton.onClick.RemoveListener(InvokeOnCloseButtonClicked);
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
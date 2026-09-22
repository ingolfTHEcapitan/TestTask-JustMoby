using System;
using System.Collections.Generic;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Services.IAP;
using _Project.Scripts.UI.Windows.Shop.Item;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Factory
{
    public class ShopItemUIFactory: IDisposable
    {
        private readonly IInstantiator _container;
        private readonly IAssetProvider _assetProvider;
        
        private readonly List<ShopItemPresenter> _presenters = new List<ShopItemPresenter>();
        
        public ShopItemUIFactory(IInstantiator container, IAssetProvider assetProvider)
        {
            _container = container;
            _assetProvider = assetProvider;
        }

        public void Dispose()
        {
            foreach (var presenter in _presenters)
                presenter.Dispose();
            
            _presenters.Clear();
        }

        public async UniTask<ShopItemView> CreateShopItemAsync(Transform parent, ProductDescription productDescription, AudioSource audioSource)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.ShopItem);
            ShopItemView view = _container.InstantiatePrefabForComponent<ShopItemView>(prefab, parent);

            ShopItemModel model = _container.Instantiate<ShopItemModel>(new object[] {productDescription});
            ShopItemPresenter presenter = _container.Instantiate<ShopItemPresenter>(new object[] {view, model});
            await presenter.Initialize(audioSource);
            
            _presenters.Add(presenter);
            
            return view;
        }
    }
}
using System;
using System.Collections.Generic;
using _Project.Scripts.Configs.IAP;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.Services.IAP
{
    public interface IIAPService: IDisposable, IInitializable
    {
        bool IsInitialized { get; }
        UniTask<bool> StartPurchase(ProductDescription productDescription);
        List<ProductDescription> GetProducts();
    }
}
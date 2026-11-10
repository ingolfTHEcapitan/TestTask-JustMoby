using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Infrastructure.AssetManagement
{
    public interface IAssetProvider
    {
        UniTask<T> LoadAsync<T>(AssetReference assetReference, bool isGlobal = false) where T : UnityEngine.Object;
        UniTask<T> LoadAsync<T>(string assetAddress, bool isGlobal = false) where T : UnityEngine.Object;
        UniTask InitializeAsync();
    }
}
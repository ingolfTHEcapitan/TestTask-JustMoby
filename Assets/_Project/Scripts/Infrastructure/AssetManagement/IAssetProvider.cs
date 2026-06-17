using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Infrastructure.AssetManagement
{
    public interface IAssetProvider
    {
        UniTask<T> LoadAsync<T>(AssetReference assetReference) where T : class;
        UniTask<T> LoadAsync<T>(string assetAddress) where T : class;
        void CleanUp();
        UniTask InitializeAsync();
    }
}
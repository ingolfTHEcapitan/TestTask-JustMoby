using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Infrastructure.AssetManagement
{
    public interface IAssetProvider
    {
        Task<T> LoadAsync<T>(AssetReference assetReference) where T : class;
        Task<T> LoadAsync<T>(string assetAddress) where T : class;
        void CleanUp();
    }
}
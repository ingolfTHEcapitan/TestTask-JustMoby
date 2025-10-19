using UnityEngine;

namespace _Project._Scripts.Infrastructure.Services.AssetManagement
{
    public interface IAssetProvider
    {
        GameObject Instantiate(string path, Vector3 at);
        GameObject Instantiate(string path, Transform parent);
    }
}
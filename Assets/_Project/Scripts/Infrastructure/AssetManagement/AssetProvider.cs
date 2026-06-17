using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Scripts.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        private readonly Dictionary<string, AsyncOperationHandle> _completedCache = new Dictionary<string, AsyncOperationHandle>();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new Dictionary<string, List<AsyncOperationHandle>>();
        private AsyncOperationHandle<IResourceLocator> _asyncOperationHandle;

        public async UniTask InitializeAsync()
        {
            _asyncOperationHandle = Addressables.InitializeAsync();
            await _asyncOperationHandle.ToUniTask();
        }

        public async UniTask<T> LoadAsync<T>(AssetReference assetReference) where T : class
        {
            if (_completedCache.TryGetValue(assetReference.AssetGUID, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetReference);
            return await RunWithCacheOnComplete(handle, assetReference.AssetGUID);
        }
        
        public async UniTask<T> LoadAsync<T>(string assetAddress) where T : class
        {
            if (!_asyncOperationHandle.IsDone) 
                await _asyncOperationHandle.ToUniTask();
            
            if (_completedCache.TryGetValue(assetAddress, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetAddress);
            return await RunWithCacheOnComplete(handle, assetAddress);
        }

        public void CleanUp()
        {
            foreach (List<AsyncOperationHandle> resourceHandle in _handles.Values)
                foreach (AsyncOperationHandle handle in resourceHandle)
                    Addressables.Release(handle);
            
            _completedCache.Clear();
            _handles.Clear();
        }

        private async UniTask<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey) where T : class
        {
            handle.Completed += completeHandle => 
                _completedCache[cacheKey] = completeHandle;
            
            AddHandle(cacheKey, handle);
            return await handle.ToUniTask();
        }
        
        private void AddHandle<T>(string key, AsyncOperationHandle<T> handle) where T : class
        {
            if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles))
            {
                resourceHandles = new List<AsyncOperationHandle>();
                _handles[key] = resourceHandles;
            }

            resourceHandles.Add(handle);
        }
    }
}
using System;
using System.Collections.Generic;
using _Project.Scripts.Services.SceneLoader;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Scripts.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider, IDisposable
    {
        private readonly ISceneLoaderService _sceneLoader;
        private readonly Dictionary<string, AsyncOperationHandle> _completedCache = new Dictionary<string, AsyncOperationHandle>();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _sceneHandles = new Dictionary<string, List<AsyncOperationHandle>>();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _globalHandles = new Dictionary<string, List<AsyncOperationHandle>>();

        private AsyncOperationHandle<IResourceLocator> _asyncOperationHandle;

        public AssetProvider(ISceneLoaderService sceneLoader) => 
            _sceneLoader = sceneLoader;
        
        public async UniTask InitializeAsync()
        {
            _sceneLoader.BeforeSceneUnload += CleanUpSceneCache;
            
            _asyncOperationHandle = Addressables.InitializeAsync();
            await _asyncOperationHandle.ToUniTask();
        }

        public void Dispose() => 
            _sceneLoader.BeforeSceneUnload -= CleanUpSceneCache;

        public async UniTask<T> LoadAsync<T>(AssetReference assetReference, bool isGlobal = false) where T : class
        {
            if (!_asyncOperationHandle.IsDone) 
                await _asyncOperationHandle.ToUniTask();
            
            if (_completedCache.TryGetValue(assetReference.AssetGUID, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetReference);
            return await RunWithCacheOnCompleteAsync(assetReference.AssetGUID, handle, isGlobal);
        }

        public async UniTask<T> LoadAsync<T>(string assetAddress, bool isGlobal = false) where T : class
        {
            if (!_asyncOperationHandle.IsDone) 
                await _asyncOperationHandle.ToUniTask();
            
            if (_completedCache.TryGetValue(assetAddress, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetAddress);
            return await RunWithCacheOnCompleteAsync(assetAddress, handle, isGlobal);
        }

        private async UniTask<T> RunWithCacheOnCompleteAsync<T>(string cacheKey, AsyncOperationHandle<T> handle, 
            bool isGlobal) where T : class
        {
            handle.Completed += completeHandle => 
                _completedCache[cacheKey] = completeHandle;
            
            AddHandle(cacheKey, handle, isGlobal);
            return await handle.ToUniTask();
        }

        private void AddHandle<T>(string key, AsyncOperationHandle<T> handle, bool isGlobal) where T : class
        {
            Dictionary<string, List<AsyncOperationHandle>> targetDictionary = isGlobal ? _globalHandles : _sceneHandles;

            if (!targetDictionary.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles))
            {
                resourceHandles = new List<AsyncOperationHandle>();
                targetDictionary[key] = resourceHandles;
            }

            resourceHandles.Add(handle);
        }

        private void CleanUpSceneCache()
        {
            foreach (List<AsyncOperationHandle> resourceHandle in _sceneHandles.Values)
            foreach (AsyncOperationHandle handle in resourceHandle)
                Addressables.Release(handle);

            foreach (string sceneKey in _sceneHandles.Keys)
                _completedCache.Remove(sceneKey);
            
            _sceneHandles.Clear();
        }
    }
}
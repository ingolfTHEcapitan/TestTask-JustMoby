using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine;

namespace _Project.Scripts.Services.RemoteConfig 
{
    public class RemoteConfigService : IRemoteConfigService
    {
        public FirebaseRemoteConfig RemoteConfig { get; private set; }
        
        public async UniTask FetchDataAsync()
        {
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;

            try
            {
                await remoteConfig.FetchAsync(TimeSpan.Zero);
            }
            catch (Exception e)
            {
                Debug.LogError($"[REMOTE CONFIG] Fetch request threw exception: {e.Message}");
                Debug.LogException(e);
                return;
            }
            
            ConfigInfo info = remoteConfig.Info;
            if (info.LastFetchStatus != LastFetchStatus.Success)
            {
                Debug.LogError(
                    "[REMOTE CONFIG] Fetch failed. " +
                    $"Status: {info.LastFetchStatus}, " +
                    $"Reason: {info.LastFetchFailureReason}, " +
                    $"FetchTime: {info.FetchTime:g}");
                return;
            }

            try
            {
                await remoteConfig.ActivateAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[REMOTE CONFIG] Activation failed, message {e.Message}");
                Debug.LogException(e);
                return;
            }
            
            RemoteConfig = remoteConfig;
        }
    }
}
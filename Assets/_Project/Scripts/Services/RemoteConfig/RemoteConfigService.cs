using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Configs;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Configs.Weapon;
using Firebase.RemoteConfig;
using UnityEngine;

namespace _Project.Scripts.Services.RemoteConfig 
{
    public class RemoteConfigService : IRemoteConfigService
    {
        public FirebaseRemoteConfig RemoteConfigInstance { get; private set; }
        
        public async Task FetchDataAsync()
        {
            Debug.Log("Fetching data...");

            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
            await fetchTask;
            
            if (!fetchTask.IsCompleted)
            {
                Debug.LogError("Retrieval hasn't finished.");
                return;
            }
            
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            ConfigInfo info = remoteConfig.Info;
            if (info.LastFetchStatus != LastFetchStatus.Success)
            {
                Debug.LogError($"fetchComplete was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
                return;
            }

            await remoteConfig.ActivateAsync();

            Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
            RemoteConfigInstance = remoteConfig;
        }
    }
}
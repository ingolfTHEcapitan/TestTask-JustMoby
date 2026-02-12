using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;
using Zenject;
namespace _Project.Scripts.Configs 
{
    public class RemoteConfig: IInitializable
{
    private int _test;

    public void Initialize()
    {
        FetchDataAsync();
    }

    public Task FetchDataAsync()
    {
        Debug.Log("Fetching data...");

        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }


    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync().ContinueWithOnMainThread(
        task =>
        {
            Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
            _test = (int)remoteConfig.GetValue("testValue").LongValue;
            Debug.Log("testValue " + _test);
        });
    }
}
}
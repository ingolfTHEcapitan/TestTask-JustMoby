using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine;

namespace _Project.Scripts.Services.RemoteConfig
{
    public interface IRemoteConfigService
    {
        Task FetchDataAsync();
        FirebaseRemoteConfig RemoteConfigInstance { get; }
    }
}
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;

namespace _Project.Scripts.Services.RemoteConfig
{
    public interface IRemoteConfigService
    {
        UniTask FetchDataAsyncAsync();
        FirebaseRemoteConfig RemoteConfig { get; }
    }
}
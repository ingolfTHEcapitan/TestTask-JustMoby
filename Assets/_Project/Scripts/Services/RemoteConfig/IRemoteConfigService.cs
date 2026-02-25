using System.Threading.Tasks;
using Firebase.RemoteConfig;

namespace _Project.Scripts.Services.RemoteConfig
{
    public interface IRemoteConfigService
    {
        Task FetchDataAsync();
        FirebaseRemoteConfig RemoteConfig { get; }
    }
}
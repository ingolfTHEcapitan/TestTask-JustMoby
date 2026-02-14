using System.Threading.Tasks;

namespace _Project.Scripts.Services.RemoteConfig
{
    public interface IRemoteConfigService
    {
        Task FetchDataAsync();
    }
}
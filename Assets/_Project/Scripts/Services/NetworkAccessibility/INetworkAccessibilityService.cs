using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.NetworkAccessibility
{
    public interface INetworkAccessibilityService
    {
        UniTask<bool> CheckNetworkConnectionAsync();
    }
}
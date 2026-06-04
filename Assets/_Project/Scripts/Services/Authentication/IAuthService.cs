using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.Authentication
{
    public interface IAuthService
    {
        UniTask SignUpAsync();
        bool IsSignedIn { get; }
    }
}
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.LoadingCurtain
{
    public interface ILoadingCurtainService
    {
        UniTask ShowLoadingAsync();
        void HideLoading();
    }
}
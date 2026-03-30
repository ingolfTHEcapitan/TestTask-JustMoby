using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.LoadingScreen
{
    public interface ILoadingCurtainService
    {
        UniTask ShowLoading();
        void HideLoading();
    }
}
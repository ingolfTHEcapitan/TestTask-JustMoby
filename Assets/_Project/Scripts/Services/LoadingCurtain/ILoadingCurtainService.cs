using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.LoadingCurtain
{
    public interface ILoadingCurtainService
    {
        UniTask ShowLoading();
        void HideLoading();
    }
}
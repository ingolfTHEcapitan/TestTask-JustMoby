using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.LoadingCurtain
{
    public interface ILoadingCurtainService
    {
        UniTask ShowLoadingAsync();
        void HideLoading();
    }
}
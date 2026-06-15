using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.LoadingCurtain.Factory
{
    public interface ILoadingCurtainFactory
    {
        UniTask<UI.LoadingCurtain> CreateLoadingCurtain();
    }
}
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.LoadingCurtain.Factory
{
    public interface ILoadingCurtainFactory
    {
        UniTask<LoadingCurtain> CreateLoadingCurtainAsync();
    }
}
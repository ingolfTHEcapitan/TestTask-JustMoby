using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.Factory.LoadingCurtainFactory
{
    public interface ILoadingCurtainFactory
    {
        UniTask<LoadingCurtain> CreateLoadingCurtain();
    }
}
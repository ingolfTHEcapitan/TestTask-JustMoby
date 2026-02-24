using System.Threading.Tasks;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.Services.Factory.LoadingCurtainFactory
{
    public interface ILoadingCurtainFactory
    {
        Task<LoadingCurtain> CreateLoadingCurtain();
    }
}
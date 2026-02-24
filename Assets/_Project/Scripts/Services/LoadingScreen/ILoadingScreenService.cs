using System.Threading.Tasks;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.Services.LoadingScreen
{
    public interface ILoadingScreenService
    {
        Task<LoadingCurtain> ShowLoading();
        void HideLoading();
    }
}
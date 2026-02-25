using System.Threading.Tasks;

namespace _Project.Scripts.Services.LoadingScreen
{
    public interface ILoadingCurtainService
    {
        Task ShowLoading();
        void HideLoading();
    }
}
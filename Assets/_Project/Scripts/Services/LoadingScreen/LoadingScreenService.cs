using System.Threading.Tasks;
using _Project.Scripts.Services.Factory.LoadingCurtainFactory;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.Services.LoadingScreen
{
    public class LoadingScreenService : ILoadingScreenService
    {
        private readonly ILoadingCurtainFactory _curtainFactory;
        
        private LoadingCurtain _loadingCurtain;

        public LoadingScreenService(ILoadingCurtainFactory curtainFactory) => 
            _curtainFactory = curtainFactory;

        public async Task<LoadingCurtain> ShowLoading()
        {
            if (_loadingCurtain == null) 
                _loadingCurtain = await _curtainFactory.CreateLoadingCurtain();
            
            _loadingCurtain.Show();
            
            return _loadingCurtain;
        }
        
        public void HideLoading()
        {
            if (_loadingCurtain != null) 
                _loadingCurtain.Hide();
        }
    }
}
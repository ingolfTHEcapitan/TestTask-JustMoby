using _Project.Scripts.Services.LoadingCurtain.Factory;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.LoadingCurtain
{
    public class LoadingCurtainService : ILoadingCurtainService
    {
        private readonly ILoadingCurtainFactory _curtainFactory;
        
        private UI.LoadingCurtain _loadingCurtain;

        public LoadingCurtainService(ILoadingCurtainFactory curtainFactory) => 
            _curtainFactory = curtainFactory;

        public async UniTask ShowLoading()
        {
            if (_loadingCurtain == null) 
                _loadingCurtain = await _curtainFactory.CreateLoadingCurtain();
            
            _loadingCurtain.Show();
        }
        
        public void HideLoading()
        {
            if (_loadingCurtain != null) 
                _loadingCurtain.Hide();
        }
    }
}
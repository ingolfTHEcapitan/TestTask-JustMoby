using _Project.Scripts.UI.Factory;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.LoadingCurtain
{
    public class LoadingCurtainService : ILoadingCurtainService
    {
        private readonly IUIFactory _uiFactory;
        
        private LoadingCurtain _loadingCurtainView;

        public LoadingCurtainService(IUIFactory uiFactory) => 
            _uiFactory = uiFactory;

        public async UniTask ShowLoadingAsync()
        {
            if (_loadingCurtainView == null) 
                _loadingCurtainView = await _uiFactory.CreateLoadingCurtainAsync();
            
            _loadingCurtainView.Open();
        }
        
        public void HideLoading()
        {
            if (_loadingCurtainView != null) 
                _loadingCurtainView.Close();
        }
    }
}
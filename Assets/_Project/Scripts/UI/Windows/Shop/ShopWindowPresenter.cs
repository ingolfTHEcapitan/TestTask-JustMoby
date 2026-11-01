using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.Shop
{
    public class ShopWindowPresenter
    {
        private readonly ShopWindowView _view;
        private readonly ShopWindowModel _model;
        
        private bool IapIsInitialized => _model.IapIsInitialized;
        
        public ShopWindowPresenter(ShopWindowView view, ShopWindowModel model)
        {
            _view = view;
            _model = model;
        }

        public async UniTask InitializeAsync()
        {
            _view.Initialize();
            _view.OnWindowDestroy += CleanUp;
            _view.OnCloseButtonClicked += Close;
            await RefreshAvailableItemsAsync();
        }

        public async UniTask OpenAsync()
        {
            await RefreshAvailableItemsAsync();
            await _view.OpenAsync();
        }

        private void CleanUp()
        {
            _view.OnWindowDestroy -= CleanUp;
            _view.OnCloseButtonClicked -= Close;
        }

        private void Close() => 
            _view.CloseAsync().Forget();

        private async UniTask RefreshAvailableItemsAsync() => 
            await _view.RefreshAvailableItemsAsync(IapIsInitialized, _model.GetProducts());
    }
}
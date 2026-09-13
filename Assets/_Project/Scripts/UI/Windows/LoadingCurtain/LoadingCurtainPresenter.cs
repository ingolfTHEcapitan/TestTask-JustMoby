namespace _Project.Scripts.UI.Windows.LoadingCurtain
{
    public class LoadingCurtainPresenter
    {
        private readonly LoadingCurtainView _view;

        public LoadingCurtainPresenter(LoadingCurtainView view) => 
            _view = view;

        public void ShowLoading() => 
            _view.Open();

        public void HideLoading() => 
            _view.Close();
    }
}
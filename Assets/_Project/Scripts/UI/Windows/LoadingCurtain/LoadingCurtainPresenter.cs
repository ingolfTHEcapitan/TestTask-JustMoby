using _Project.Scripts.UI.Common;

namespace _Project.Scripts.UI.Windows.LoadingCurtain
{
    public class LoadingCurtainPresenter
    {
        private readonly LoadingCurtainView _view;
        private CursorController _cursorController;

        public LoadingCurtainPresenter(LoadingCurtainView view, CursorController cursorController)
        {
            _view = view;
            _cursorController = cursorController;
        }

        public void ShowLoading()
        {
            _view.Open();
            _cursorController.SetCursorVisible(false);
        }

        public void HideLoading() => 
            _view.Close();
    }
}
namespace _Project.Scripts.UI.Windows.GameOver
{
    public class GameOverWindowPresenter
    {
        private GameOverWindowView _view;
        private GameOverWindowModel _model;

        public GameOverWindowPresenter(GameOverWindowView view, GameOverWindowModel model)
        {
            _model = model;
            _view = view;
        }
    }
}
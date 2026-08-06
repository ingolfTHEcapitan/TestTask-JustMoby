using _Project.Scripts.UI.Common;

namespace _Project.Scripts.Services.GamePause
{
    public class GamePauseService : IGamePauseService
    {
        public bool IsPaused { get; private set; }
        
        private readonly CursorController _cursorController;

        public GamePauseService(CursorController cursorController) => 
            _cursorController = cursorController;

        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            _cursorController.SetCursorVisible(paused);
        }
    }
}
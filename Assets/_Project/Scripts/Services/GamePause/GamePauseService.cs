using _Project.Scripts.Logic.Common;

namespace _Project.Scripts.Services.GamePause
{
    public class GamePauseService : IGamePauseService
    {
        public bool IsPaused { get; private set; }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            CursorController.SetCursorVisible(paused);
        }
    }
}
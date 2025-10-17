using _Project._Scripts.Logic;

namespace _Project._Scripts.Infrastructure.Services.GamePause
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
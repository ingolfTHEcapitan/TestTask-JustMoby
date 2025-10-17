namespace _Project._Scripts.Services.GamePause
{
    public interface IGamePauseService
    {
        bool IsPaused { get; }
        void SetPaused(bool paused);
    }
}
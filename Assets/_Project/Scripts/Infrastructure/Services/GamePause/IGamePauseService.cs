namespace _Project.Scripts.Infrastructure.Services.GamePause
{
    public interface IGamePauseService
    {
        bool IsPaused { get; }
        void SetPaused(bool paused);
    }
}
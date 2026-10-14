using System;

namespace _Project.Scripts.Services.GamePause
{
    public interface IGamePauseService : IPauseHandler
    {
        event Action OnPausableUpdate;
        bool IsPaused { get; }
        void Register(IPauseHandler handler);
        void UnRegister(IPauseHandler handler);
    }
}
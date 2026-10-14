
using System;
using System.Collections.Generic;
using Zenject;

namespace _Project.Scripts.Services.GamePause
{
    public class GamePauseService : IGamePauseService, ITickable
    {
        public event Action OnPausableUpdate;
        
        private readonly List<IPauseHandler> _handlers = new List<IPauseHandler>();

        public void Register(IPauseHandler handler) => 
            _handlers.Add(handler);

        public void UnRegister(IPauseHandler handler) => 
            _handlers.Remove(handler);

        public bool IsPaused { get; private set; }
        
        public void SetPaused(bool paused)
        {
            IsPaused = paused;

            foreach (IPauseHandler handle in _handlers) 
                handle.SetPaused(paused);
        }

        public void Tick()
        {
            if (IsPaused)
                return;
            
            OnPausableUpdate?.Invoke();
        }
    }
}
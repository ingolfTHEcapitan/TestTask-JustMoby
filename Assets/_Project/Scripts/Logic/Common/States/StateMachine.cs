using System;
using System.Collections.Generic;

namespace _Project.Scripts.Logic.Common.States
{
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IState> _states = new Dictionary<Type, IState>();
        private IState _currentState;

        public void RegisterState(IState state) => 
            _states.Add(state.GetType(), state);

        public void SetState<TState>() where TState : IState
        {
            Type type = typeof(TState);

            if (_currentState?.GetType() == type)
                return;
            
            if (_states.TryGetValue(type, out IState newState))
            {
                _currentState?.Exit();
                _currentState = newState;
                _currentState?.Enter();
            }
        }

        public void Update() => 
            _currentState?.Update();

        public void InvokeDisposeOnStates()
        {
            foreach (IState state in _states.Values)
            {
                if (state is IDisposable disposable) 
                    disposable.Dispose();
            }
        }
    }
}
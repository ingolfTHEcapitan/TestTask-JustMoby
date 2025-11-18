namespace _Project.Scripts.Logic.Common.States
{
    public interface IStateMachine
    {
        void RegisterState(IState state);
        void SetState<TState>() where TState : IState;
        void Update();
        void InvokeDisposeOnStates();
    }
}
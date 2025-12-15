namespace _Project.Scripts.Logic.Common.StateMachine.States
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
        void Update();
    }
}
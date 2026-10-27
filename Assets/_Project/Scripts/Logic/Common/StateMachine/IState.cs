namespace _Project.Scripts.Logic.Common.StateMachine
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
        void Update();
    }
}
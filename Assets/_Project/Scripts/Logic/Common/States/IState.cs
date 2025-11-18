namespace _Project.Scripts.Logic.Common.States
{
    public interface IState
    {
        void Enter();
        void Exit();
        void Update();
    }
}
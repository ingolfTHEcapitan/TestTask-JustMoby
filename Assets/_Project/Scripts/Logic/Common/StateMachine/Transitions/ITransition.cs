using _Project.Scripts.Logic.Common.StateMachine.States;

namespace _Project.Scripts.Logic.Common.StateMachine.Transitions
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}
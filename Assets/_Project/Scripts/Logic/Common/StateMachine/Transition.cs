using _Project.Scripts.Logic.Common.StateMachine.Predicates;

namespace _Project.Scripts.Logic.Common.StateMachine
{
    public class Transition
    {
        public IState To { get; }
        public IPredicate Condition { get; }

        public Transition(IState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
    }
}
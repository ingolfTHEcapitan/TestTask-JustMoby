namespace _Project.Scripts.Logic.Common.States
{
    public class State : IState
    {
        private IStateMachine _stateMachine;

        protected State(IStateMachine stateMachine) => 
            _stateMachine = stateMachine;

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
    }
}
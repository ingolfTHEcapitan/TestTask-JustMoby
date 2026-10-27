using System;
using System.Collections.Generic;
using _Project.Scripts.Logic.Common.StateMachine.Predicates;

namespace _Project.Scripts.Logic.Common.StateMachine
{
    public class StateMachine
    {
        private StateNode _currentStateNode;
        private readonly Dictionary<Type, StateNode> _nodes = new Dictionary<Type, StateNode>();
        private readonly HashSet<Transition> _anyTransitions = new HashSet<Transition>();
        
        public IState CurrentState => _currentStateNode.State;

        public void Update()
        {
            Transition transition = GetTransition();
            
            if (transition != null)
                ChangeState(transition.To);
            
            _currentStateNode.State.Update();
        }
        
        public void SetState(IState state)
        {
            _currentStateNode = _nodes[state.GetType()];
            _currentStateNode.State.OnEnter();
        }
        
        public IState GetState<TState>() where TState: class, IState => 
            _nodes[typeof(TState)].State as TState;
        
        public void AddTransition(IState from, IState to, IPredicate condition) => 
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        
        public void AddAnyTransition(IState to, IPredicate condition) => 
            _anyTransitions.Add(new Transition(GetOrAddNode(to).State , condition));
        
        private void ChangeState(IState state)
        {
            if (_currentStateNode.State == state)
                return;
            
            IState previousState = _currentStateNode.State;
            IState nextState = _nodes[state.GetType()].State;
            
            previousState?.OnExit();
            nextState?.OnEnter();
            _currentStateNode = _nodes[state.GetType()];
        }

        private Transition GetTransition()
        {
            foreach (Transition transition in _anyTransitions)
                if (transition.Condition.Evaluate())
                    return transition;
            
            foreach (Transition transition in _currentStateNode.Transitions)
                if (transition.Condition.Evaluate())
                    return transition;
            
            return null;
        }
        
        private StateNode GetOrAddNode(IState state)
        {
            StateNode node = _nodes.GetValueOrDefault(state.GetType());

            if (node == null)
            {
                node = new StateNode(state);
                _nodes.Add(state.GetType(), node);
            }
            
            return node;
        }
        
        class StateNode
        {
            public IState State { get; }
            
            public HashSet<Transition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<Transition>();
            }

            public void AddTransition(IState to, IPredicate condition) => 
                Transitions.Add(new Transition(to, condition));
        }
    }
}
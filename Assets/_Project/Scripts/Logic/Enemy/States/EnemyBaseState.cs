using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common.StateMachine.States;
using UnityEngine.AI;

namespace _Project.Scripts.Logic.Enemy.States
{
    public abstract class EnemyBaseState : IState
    {
        protected readonly NavMeshAgent _agent;
        protected readonly EnemyConfig _config;

        protected EnemyBaseState(NavMeshAgent agent, EnemyConfig config)
        {
            _config = config;
            _agent = agent;
        }

        public virtual void OnEnter() { }

        public virtual void OnExit() { }

        public virtual void Update() { }
    }
}
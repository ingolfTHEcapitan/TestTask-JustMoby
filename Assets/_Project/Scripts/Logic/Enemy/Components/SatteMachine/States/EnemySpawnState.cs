using _Project.Scripts.Configs;
using UnityEngine.AI;

namespace _Project.Scripts.Logic.Enemy.Components.SatteMachine.States
{
    public class EnemySpawnState : EnemyBaseState
    {
        public EnemySpawnState(NavMeshAgent agent, EnemyConfig config) : base(agent, config)
        {
        }
    }
}
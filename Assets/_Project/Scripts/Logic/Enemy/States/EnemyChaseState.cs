using _Project.Scripts.Configs;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Logic.Enemy.States
{
    public class EnemyChaseState : EnemyBaseState
    {
        private readonly Transform _playerTransform;
        private readonly EnemyRotateToPlayer _enemyRotateToPlayer;

        public EnemyChaseState(NavMeshAgent agent, EnemyConfig config, Transform playerTransform, 
            EnemyRotateToPlayer enemyRotateToPlayer) : base(agent, config)
        {
            _playerTransform = playerTransform;
            _enemyRotateToPlayer = enemyRotateToPlayer;
        }

        public override void OnEnter()
        {
            _agent.ResetPath();
            _enemyRotateToPlayer.enabled = true;
        }

        public override void Update() => 
            _agent.SetDestination(_playerTransform.position);

        public override void OnExit() => 
            _enemyRotateToPlayer.enabled = false;
    }
}
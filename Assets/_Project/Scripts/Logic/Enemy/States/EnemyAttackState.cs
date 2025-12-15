using System;
using System.Linq;
using _Project.Scripts.Configs;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Common.StateMachine.Transitions;
using _Project.Scripts.Logic.Tools;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Logic.Enemy.States
{
    public class EnemyAttackState : EnemyBaseState
    {
        private const string PlayerLayer = "Player";
        private const float AttackYOffset = 0.5f;
        private const float DebugLifeTime = 3;

        public event Action OnAttackStarted;

        private readonly Transform _enemyTransform;
        private readonly EnemyRotateToPlayer _enemyRotateToPlayer;
        private readonly IPredicate _attackCooldownIsUp;
        
        private readonly Collider[] _hits = new Collider[1];
        private readonly int _layerMask;

        private bool _isAttacking;

        public EnemyAttackState(NavMeshAgent agent, EnemyConfig config, EnemyRotateToPlayer enemyRotateToPlayer, 
            IPredicate attackCooldownIsUp,Transform enemyTransform) : base(agent, config)
        {
            _attackCooldownIsUp = attackCooldownIsUp;
            _enemyRotateToPlayer = enemyRotateToPlayer;
            _enemyTransform = enemyTransform;

            _layerMask = LayerMask.GetMask(PlayerLayer);
        }

        public override void OnEnter()
        {
            _agent.ResetPath();
            _enemyRotateToPlayer.enabled = true;
        }

        public override void Update()
        {
            if (CanAttack())
                StartAttack();
        }

        public override void OnExit() => 
            _enemyRotateToPlayer.enabled = false;

        private void StartAttack()
        {
            _isAttacking = true;
            OnAttackStarted?.Invoke();
        }

        public void DealDamage()
        {
            PhysicsDebug.DrawDebugSphere(GetStartPoint(), _config.AttackRadius, DebugLifeTime, Color.red);
            if (Hit(out Collider hit))
            {
                PhysicsDebug.DrawDebugSphere(GetStartPoint(), _config.AttackRadius, DebugLifeTime, Color.green);

                IHealth playerHealth = hit.GetComponent<IHealth>();
                playerHealth.TakeDamage(_config.AttackDamage);
            }
        }
        
        public void AttackEnded() =>
            _isAttacking = false;

        private bool Hit(out Collider hit)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(GetStartPoint(), _config.AttackRadius, _hits, _layerMask);
            hit = _hits.FirstOrDefault();
            return hitCount > 0;
        }

        private Vector3 GetStartPoint()
        {
            return new Vector3(_enemyTransform.position.x, _enemyTransform.position.y + AttackYOffset, _enemyTransform.position.z)
                   + _enemyTransform.forward * _config.AttackDistance;
        }

        private bool CanAttack() =>
            !_isAttacking && _attackCooldownIsUp.Evaluate();
    }
}
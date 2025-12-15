using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Enemy.States;
using UnityEngine;

namespace _Project.Scripts.Logic.Enemy
{
    public class EnemyAnimator: MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Health _health;
        [SerializeField] private EnemyDeath _death;
        
        private readonly int _dieHash = Animator.StringToHash("Die");
        private readonly int _hitHash = Animator.StringToHash("Hit");
        private readonly int _attackHash = Animator.StringToHash("Attack");
        
        private EnemyAttackState _attackState;

        public void Construct(EnemyAttackState attackState) => 
            _attackState = attackState;

        public void Initialize()
        {
            _health.OnHealthChanged += PlayHit;
            _health.OnZeroHealth += PlayDeath;
            _attackState.OnAttackStarted += PlayAttack;
        }

        private void OnDestroy()
        {
            _health.OnHealthChanged -= PlayHit;
            _health.OnZeroHealth -= PlayDeath;
            _attackState.OnAttackStarted -= PlayAttack;
        }

        private void PlayHit() => 
            _animator.SetTrigger(_hitHash);

        private void PlayDeath() => 
            _animator.SetTrigger(_dieHash);
        
        private void PlayAttack() => 
            _animator.SetTrigger(_attackHash);
    }
}
using System;
using _Project.Scripts.Configs;
using _Project.Scripts.Enemy.States;
using _Project.Scripts.Infrastructure.Services.GamePause;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Common.States;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Enemy
{
    public class EnemyStateMachine: MonoBehaviour
    {
        public event Action AttackEnded;
        public event Action Attack;
        public event Action SpawnAnimationEnded;
        
        [SerializeField] private TriggerObserver _triggerObserver;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;

        private IStateMachine _stateMachine;
        private IGamePauseService _pauseService;
        private EnemyConfig _config;
        
        [Inject]
        public void Construct(IGamePauseService pauseService, EnemyConfig config)
        {
            _pauseService = pauseService;
            _config = config;
        }
        
        public void Initialize(Vector3 spawnPoint, Transform playerTransform)
        {
            _stateMachine = new StateMachine();
            _stateMachine.RegisterState( new EnemyPatrolState(_stateMachine, enemy: this, _agent, _config, spawnPoint, _triggerObserver));
            _stateMachine.RegisterState( new EnemyAttackState(_stateMachine, enemy: this, _agent, _config, playerTransform, _triggerObserver, _animator));
            
            _stateMachine.SetState<EnemyPatrolState>();
        }

        private void Update()
        { 
            if (_pauseService.IsPaused)
                return;
            
            _stateMachine.Update();
        }

        private void OnDestroy() => 
            _stateMachine.InvokeDisposeOnStates();

        [UsedImplicitly]
        public void OnAttackEnded() => 
            AttackEnded?.Invoke();

        [UsedImplicitly]
        public void OnAttack() => 
            Attack?.Invoke();
        
        [UsedImplicitly]
        public void OnSpawnAnimationEnded() => 
            SpawnAnimationEnded?.Invoke();
        
    }
}
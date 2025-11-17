using System;
using System.Linq;
using _Project.Scripts.Configs;
using _Project.Scripts.Infrastructure.Services.GamePause;
using _Project.Scripts.Logic.Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Enemy
{
    public class EnemyAttack : MonoBehaviour
    {
        private const string PlayerLayer = "Player";
        private const float AttackYOffset = 0.5f;

        [SerializeField] private Animator _animator;
        [SerializeField] private NavMeshAgent _agent;

        private IGamePauseService _pauseService;
        private EnemyConfig _config;
        private Transform _playerTransform;
        private float _currentCooldown;
        private bool _attackIsActive;
        private bool _isAttacking;
        private int _layerMask;

        private readonly Collider[] _hits = new Collider[1];
        private readonly int _attackHash = Animator.StringToHash("Attack");

        [Inject]
        public void Construct(IGamePauseService pauseService, EnemyConfig config)
        {
            _pauseService = pauseService;
            _config = config;
        }

        public void Initialize(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            _layerMask = LayerMask.GetMask(PlayerLayer);
        }

        private void Update()
        {
            if (_pauseService.IsPaused)
                return;

            if (_attackIsActive) 
                _agent.SetDestination(_playerTransform.position);
            
            UpdateCoolDown();

            if (CanAttack()) 
                StartAttack();
        }
        
        [UsedImplicitly]
        public void OnAttack()
        {
            DrawDebugSphere(GetStartPoint(), _config.AttackRadius, 3, Color.red);
            if (Hit(out Collider hit))
            {
                DrawDebugSphere(GetStartPoint(), _config.AttackRadius, 3, Color.green);
                
                IHealth playerHealth = hit.GetComponent<IHealth>();
                playerHealth.TakeDamage(_config.AttackDamage);
            }
        }
        
        [UsedImplicitly]
        public void OnAttackEnded()
        {
            _currentCooldown = _config.AttackCooldown;
            _isAttacking = false;
        }

        public void DisableAttack()
        {
            _attackIsActive = false;
            _agent.ResetPath();
        }

        public void EnableAttack() =>
            _attackIsActive = true;

        private void StartAttack()
        {
            transform.LookAt(_playerTransform);
            _animator.SetTrigger(_attackHash);
            _isAttacking = true;
        }

        private bool Hit(out Collider hit)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(GetStartPoint(), _config.AttackRadius, _hits, _layerMask);
            hit = _hits.FirstOrDefault();
            return hitCount > 0;
        }

        private Vector3 GetStartPoint()
        {
            return new Vector3(transform.position.x, transform.position.y + AttackYOffset, transform.position.z) +
                   transform.forward * _config.AttackDistance;
        }

        private bool CanAttack() =>
            _attackIsActive && !_isAttacking && CooldownIsUp() && !_pauseService.IsPaused;

        private void UpdateCoolDown()
        {
            if (!CooldownIsUp())
                _currentCooldown -= Time.deltaTime;
        }
        
        private bool CooldownIsUp() =>
            _currentCooldown <= 0f;

        private static void DrawDebugSphere(Vector3 position, float radius, float seconds, Color color)
        {
            Debug.DrawRay(position, radius * Vector3.up, color, seconds);
            Debug.DrawRay(position, radius * Vector3.down, color, seconds);
            Debug.DrawRay(position, radius * Vector3.left, color, seconds);
            Debug.DrawRay(position, radius * Vector3.right, color, seconds);
            Debug.DrawRay(position, radius * Vector3.forward, color, seconds);
            Debug.DrawRay(position, radius * Vector3.back, color, seconds);
        }
    }
}
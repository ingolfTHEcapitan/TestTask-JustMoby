using _Project.Scripts.Logic.Common;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(EnemyAttack))]
    public class CheckAttackRange: MonoBehaviour
    {
        [SerializeField] private TriggerObserver _triggerObserver;
        [SerializeField] private EnemyAttack _enemyAttack;
        [SerializeField] private EnemyMovement _enemyMovement;
        
        private void Start()
        {
            _triggerObserver.TriggerEnter += OnTriggerEnter;
            _triggerObserver.TriggerExit += OnTriggerExit;
            
            _enemyAttack.DisableAttack();
        }

        private void OnDestroy()
        {
            _triggerObserver.TriggerEnter -= OnTriggerEnter;
            _triggerObserver.TriggerExit -= OnTriggerExit;
        }
        
        private void OnTriggerEnter(Collider obj)
        {
            _enemyAttack.EnableAttack();
            _enemyMovement.DisableMovement();
        }

        private void OnTriggerExit(Collider obj)
        {
            _enemyAttack.DisableAttack();
            _enemyMovement.EnableMovement();
        }
    }
}
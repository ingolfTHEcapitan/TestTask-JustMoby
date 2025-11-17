using UnityEngine;

namespace _Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [field: SerializeField, Range(1f, 100f)]
        public float AttackDamage { get; private set; } = 25f;

        [field: SerializeField, Range(0.5f, 10f)]
        public float AttackCooldown { get; private set; } = 3f;

        [field: SerializeField, Range(0.5f, 3f)]
        public float AttackRadius { get; private set; } = 0.5f;

        [field: SerializeField, Range(0.5f, 5f)]
        public float AttackDistance { get; private set; } = 0.5f;
        
        [field: SerializeField, Range(1f, 5f)]
        public float MoveSpeed { get; private set; } = 2.5f;
    }
}
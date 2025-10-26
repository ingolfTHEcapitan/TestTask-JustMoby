using _Project._Scripts.Logic.Weapon;
using UnityEngine;

namespace _Project._Scripts.Configs.Weapon
{
    [CreateAssetMenu(fileName = "BulletConfig", menuName = "Configs/BulletConfig")]
    public class BulletConfig : ScriptableObject
    {
        [field: SerializeField] public Bullet Prefab { get; private set; }
        [field: SerializeField] public float Speed { get; private set; } = 8f;
        [field: SerializeField] public float LifeTime { get; private set; } = 5f;
    }
}
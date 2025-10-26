using UnityEngine;

namespace _Project._Scripts.Configs.Weapon
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/WeaponConfig")]
    public class WeaponConfig : ScriptableObject
    {
        [field: SerializeField] public float FireRate { get; private set; } = 1f;
        [field: SerializeField] public BulletConfig BulletConfig { get; private set; }
    }
}
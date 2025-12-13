using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Configs.Weapon
{
    [CreateAssetMenu(fileName = "BulletConfig", menuName = "Configs/BulletConfig")]
    public class BulletConfig : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject PrefabReference { get; private set; }
        [field: SerializeField] public float Speed { get; private set; } = 8f;
        [field: SerializeField] public float LifeTime { get; private set; } = 5f;
    }
}
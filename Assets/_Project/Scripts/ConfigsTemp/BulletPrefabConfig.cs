using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.ConfigsTemp
{
    [CreateAssetMenu(fileName = "BulletPrefabConfig", menuName = "Configs/BulletPrefabConfig")]
    public class BulletPrefabConfig : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject PrefabReference { get; private set; }
    }
}
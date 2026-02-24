using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.ConfigsTemp
{
    [CreateAssetMenu(fileName = "EnemyPrefabConfig", menuName = "Configs/EnemyPrefabConfig")]
    public class EnemyPrefabConfig : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject PrefabReference { get; private set; }
    }
}
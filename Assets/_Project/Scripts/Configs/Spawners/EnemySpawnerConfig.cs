using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Configs.Spawners
{
    [CreateAssetMenu(fileName = "EnemySpawnerConfig", menuName = "Configs/EnemySpawnerConfig")]
    public class EnemySpawnerConfig : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject PrefabReference { get; private set; }
        [field: SerializeField] public int EnemiesAtTime { get; private set; }
        [field: SerializeField] public float SpawnDistance { get; private set; }
        [field: SerializeField] public float SpawnDelay { get; private set; }
    }
}
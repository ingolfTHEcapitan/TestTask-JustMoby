using UnityEngine;

namespace _Project.Scripts.Configs.Spawners
{
    [CreateAssetMenu(fileName = "EnemySpawnerConfig", menuName = "Configs/EnemySpawnerConfig")]
    public class EnemySpawnerConfig : ScriptableObject
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public int EnemiesAtTime { get; private set; }
        [field: SerializeField] public float SpawnDistance { get; private set; }
        [field: SerializeField] public float SpawnDelay { get; private set; }
    }
}
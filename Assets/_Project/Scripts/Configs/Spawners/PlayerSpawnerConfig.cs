using UnityEngine;

namespace _Project.Scripts.Configs.Spawners
{
    [CreateAssetMenu(fileName = "PlayerSpawnerConfig", menuName = "Configs/PlayerSpawnerConfig")]
    public class PlayerSpawnerConfig : ScriptableObject
    {
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public Vector3 SpawnPosition { get; private set; }
    }
}
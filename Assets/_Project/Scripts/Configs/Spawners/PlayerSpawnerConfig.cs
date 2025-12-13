using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Configs.Spawners
{
    [CreateAssetMenu(fileName = "PlayerSpawnerConfig", menuName = "Configs/PlayerSpawnerConfig")]
    public class PlayerSpawnerConfig : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject PrefabReference { get; set; }
        [field: SerializeField] public Vector3 SpawnPosition { get; private set; }
    }
}
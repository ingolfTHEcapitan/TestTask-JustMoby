using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.ConfigsTemp
{
    [CreateAssetMenu(fileName = "PlayerPrefabConfig", menuName = "Configs/PlayerPrefabConfig")]
    public class PlayerPrefabConfig : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject PrefabReference { get; set; }
    }
}
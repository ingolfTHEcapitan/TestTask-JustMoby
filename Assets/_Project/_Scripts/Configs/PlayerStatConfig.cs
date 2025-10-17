using _Project._Scripts.StatSystem;
using UnityEngine;

namespace _Project._Scripts.Configs
{
    [CreateAssetMenu(fileName = "PlayerStatConfig", menuName = "Configs/PlayerStatConfig")]
    public class PlayerStatConfig : ScriptableObject
    {
        [field: SerializeField] public StatName Name { get; private set; }
        [field: SerializeField] public float BaseValue { get; private set; }
        [field: SerializeField] public float IncrementPerLevel { get; private set; }
        [field: SerializeField] public float MaxMultiplier { get; private set; }
    }
}
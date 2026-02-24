using UnityEngine;

namespace _Project.Scripts.ConfigsTemp
{
    [CreateAssetMenu(fileName = "PlayerStatUIConfig", menuName = "Configs/PlayerStatUIConfig")]
    public class PlayerStatUIConfig : ScriptableObject
    {
        [field: SerializeField] public Sprite IconFrame { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}
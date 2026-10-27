using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player.Components;
using UnityEngine;

namespace _Project.Scripts.Logic.Player
{
    public class PlayerContainer: MonoBehaviour
    {
        [field: SerializeField] public Camera Camera { get; private set; }
        [field: SerializeField] public PlayerCameraLook CameraLook { get; private set; }
        [field: SerializeField] public PlayerVisuals Visuals { get; private set; }
        [field: SerializeField] public PlayerDeath PlayerDeath { get; private set; }
        [field: SerializeField] public PlayerMovement Movement { get; private set; }
        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public Weapon.Weapon Weapon { get; private set; }
    }
}
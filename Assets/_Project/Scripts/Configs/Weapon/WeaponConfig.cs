using System;
using UnityEngine;

namespace _Project.Scripts.Configs.Weapon
{
    [Serializable]
    public class WeaponConfig
    {
        [field: SerializeField] public float FireRate { get; private set; } = 1f;
    }
}
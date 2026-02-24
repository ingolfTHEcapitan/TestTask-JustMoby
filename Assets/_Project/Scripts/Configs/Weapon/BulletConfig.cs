using System;
using UnityEngine;

namespace _Project.Scripts.Configs.Weapon
{
    [Serializable]
    public class BulletConfig
    {
        [field: SerializeField] public float Speed { get; private set; } = 8f;
        [field: SerializeField] public float LifeTime { get; private set; } = 5f;
    }
}
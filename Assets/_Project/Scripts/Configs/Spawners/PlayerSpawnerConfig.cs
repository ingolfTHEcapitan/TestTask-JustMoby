using System;
using UnityEngine;

namespace _Project.Scripts.Configs.Spawners
{
    [Serializable]
    public class PlayerSpawnerConfig
    {
        [field: SerializeField] public Vector3 SpawnPosition { get; private set; }
    }
}
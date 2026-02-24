using System;
using UnityEngine;

namespace _Project.Scripts.Configs.Spawners
{
    [Serializable]
    public class EnemySpawnerConfig
    {
        [field: SerializeField] public int EnemiesAtTime { get; private set; }
        [field: SerializeField] public float SpawnDistance { get; private set; }
        [field: SerializeField] public float SpawnDelay { get; private set; }
    }
}
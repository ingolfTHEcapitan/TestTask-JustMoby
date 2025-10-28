using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Enemy;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factory.EnemyFactory
{
    public interface IEnemyFactory
    {
        EnemyDeath CreateEnemy(EnemySpawnerConfig config, Vector3 spawnPoint);
    }
}
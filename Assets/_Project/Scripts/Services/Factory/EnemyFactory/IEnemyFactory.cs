using System.Threading.Tasks;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Logic.Enemy;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.EnemyFactory
{
    public interface IEnemyFactory
    {
        Task<EnemyDeath> CreateEnemy(EnemySpawnerConfig config, Vector3 spawnPoint, Transform playerTransform);
    }
}
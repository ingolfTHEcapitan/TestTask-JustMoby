using System.Threading.Tasks;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.ConfigsTemp;
using _Project.Scripts.Logic.Enemy;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.EnemyFactory
{
    public interface IEnemyFactory
    {
        Task<EnemyDeath> CreateEnemy(EnemyPrefabConfig config, Vector3 spawnPoint, Transform playerTransform);
    }
}
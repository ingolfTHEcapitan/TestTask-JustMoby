using System.Threading.Tasks;
using _Project.Scripts.Logic.Enemy;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.EnemyFactory
{
    public interface IEnemyFactory
    {
        Task<EnemyDeath> CreateEnemy(Vector3 spawnPoint, Transform playerTransform);
    }
}
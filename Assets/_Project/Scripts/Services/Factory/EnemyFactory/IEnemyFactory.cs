using _Project.Scripts.Logic.Enemy;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.EnemyFactory
{
    public interface IEnemyFactory
    {
        UniTask<EnemyDeath> CreateEnemy(Vector3 spawnPoint, Transform playerTransform);
    }
}
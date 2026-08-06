using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Logic.Enemy.Factory
{
    public interface IEnemyFactory
    {
        UniTask<EnemyDeath> CreateEnemyAsync(Vector3 spawnPoint, Transform playerTransform);
    }
}
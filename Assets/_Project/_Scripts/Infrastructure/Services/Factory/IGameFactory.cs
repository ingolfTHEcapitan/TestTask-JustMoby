using _Project._Scripts.Configs;
using _Project._Scripts.Logic.Weapon;
using UnityEngine;

namespace _Project._Scripts.Infrastructure.Services.Factory
{
    public interface IGameFactory
    {
        Bullet CreateBullet(Vector3 at, Vector3 shootDirection);
        GameObject CreateEnemy(EnemySpawnerConfig config, Vector3 at);
    }
}
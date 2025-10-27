using _Project._Scripts.Configs;
using _Project._Scripts.Configs.Weapon;
using _Project._Scripts.Logic.Weapon;
using UnityEngine;

namespace _Project._Scripts.Infrastructure.Services.Factory
{
    public interface IGameFactory
    {
        Bullet CreateBullet(BulletConfig config, Transform at, Vector3 shootDirection);
        GameObject CreateEnemy(EnemySpawnerConfig config, Vector3 spawnPoint);
        GameObject CreatePlayer(GameObject prefab, Vector3 at);
        GameObject CreateHud(GameObject prefab);
        GameObject CreatePopUpLayer(GameObject prefab);
    }
}
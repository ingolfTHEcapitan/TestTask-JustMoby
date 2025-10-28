using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.Weapon;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factory
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
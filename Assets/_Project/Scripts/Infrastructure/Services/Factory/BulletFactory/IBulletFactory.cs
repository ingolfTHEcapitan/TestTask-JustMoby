using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.Weapon;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factory.BulletFactory
{
    public interface IBulletFactory
    {
        Bullet CreateBullet(BulletConfig config, Transform at, Vector3 shootDirection);
    }
}
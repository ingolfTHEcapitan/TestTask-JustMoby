using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.Weapon;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.BulletFactory
{
    public interface IBulletFactory
    {
        UniTask<Bullet> CreateBullet(BulletConfig config, Transform at, Vector3 shootDirection);
    }
}
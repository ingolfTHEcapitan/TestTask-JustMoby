using _Project.Scripts.Configs.Weapon;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Logic.Player.Weapon.Bullet.Factory
{
    public interface IBulletFactory
    {
        UniTask<Bullet> CreateBullet(BulletConfig config, Transform at, Vector3 shootDirection);
    }
}
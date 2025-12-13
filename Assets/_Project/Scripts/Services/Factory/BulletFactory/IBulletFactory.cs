using System.Threading.Tasks;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.Weapon;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.BulletFactory
{
    public interface IBulletFactory
    {
        Task<Bullet> CreateBullet(BulletConfig config, Transform at, Vector3 shootDirection);
    }
}
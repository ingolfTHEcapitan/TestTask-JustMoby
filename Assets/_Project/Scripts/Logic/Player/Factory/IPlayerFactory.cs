using _Project.Scripts.Logic.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Logic.Player.Factory
{
    public interface IPlayerFactory
    {
        UniTask<Health> CreatePlayer(Vector3 at);
    }
}
using _Project.Scripts.Logic.Common;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factory.PlayerFactory
{
    public interface IPlayerFactory
    {
        Health CreatePlayer(GameObject prefab, Vector3 at);
    }
}
using System.Threading.Tasks;
using _Project.Scripts.Logic.Common;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.PlayerFactory
{
    public interface IPlayerFactory
    {
        Task<Health> CreatePlayer(Vector3 at);
    }
}
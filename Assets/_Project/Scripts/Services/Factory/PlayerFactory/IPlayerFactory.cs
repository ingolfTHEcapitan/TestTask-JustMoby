using System.Threading.Tasks;
using _Project.Scripts.Logic.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Services.Factory.PlayerFactory
{
    public interface IPlayerFactory
    {
        Task<Health> CreatePlayer(AssetReference assetReference, Vector3 at);
    }
}
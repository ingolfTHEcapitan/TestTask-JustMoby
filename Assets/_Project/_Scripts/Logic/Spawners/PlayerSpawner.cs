using _Project._Scripts.Configs;
using _Project._Scripts.Infrastructure.Services.ConfigsManagement;
using _Project._Scripts.Infrastructure.Services.Factory;
using UnityEngine;

namespace _Project._Scripts.Logic.Spawners
{
    public class PlayerSpawner
    {
        private readonly IGameFactory _factory;
        private readonly IConfigsProvider _configs;

        public PlayerSpawner(IGameFactory factory, IConfigsProvider configs)
        {
            _factory = factory;
            _configs = configs;
        }

        public GameObject Spawn(Transform parent)
        {
            PlayerSpawnerConfig config = _configs.PlayerSpawner;
            return _factory.CreatePlayer(config.Prefab, config.SpawnPosition, parent);
        }
    }
}
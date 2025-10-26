using _Project._Scripts.Configs;
using _Project._Scripts.Infrastructure.Services.Factory;
using UnityEngine;

namespace _Project._Scripts.Logic.Spawners
{
    public class PlayerSpawner
    {
        private readonly IGameFactory _factory;
        private readonly PlayerSpawnerConfig _config;

        public PlayerSpawner(IGameFactory factory, PlayerSpawnerConfig config)
        {
            _factory = factory;
            _config = config;
        }

        public GameObject Spawn(Transform parent) => 
            _factory.CreatePlayer(_config.Prefab, _config.SpawnPosition, parent);
    }
}
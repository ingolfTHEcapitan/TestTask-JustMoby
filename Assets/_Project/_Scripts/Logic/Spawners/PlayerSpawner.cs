using _Project._Scripts.Configs;
using _Project._Scripts.Infrastructure.Services.Factory;
using JetBrains.Annotations;
using UnityEngine;

namespace _Project._Scripts.Logic.Spawners
{
    [UsedImplicitly]
    public class PlayerSpawner
    {
        private readonly IGameFactory _factory;
        private readonly PlayerSpawnerConfig _config;

        public PlayerSpawner(IGameFactory factory, PlayerSpawnerConfig config)
        {
            _factory = factory;
            _config = config;
        }

        public GameObject Spawn() => 
            _factory.CreatePlayer(_config.Prefab, _config.SpawnPosition);
    }
}
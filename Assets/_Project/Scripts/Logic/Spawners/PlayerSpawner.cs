using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Infrastructure.Services.Factory;
using UnityEngine;

namespace _Project.Scripts.Logic.Spawners
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

        public GameObject Spawn() => 
            _factory.CreatePlayer(_config.Prefab, _config.SpawnPosition);
    }
}
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Infrastructure.Services.Factory.PlayerFactory;
using _Project.Scripts.Logic.Common;

namespace _Project.Scripts.Logic.Spawners
{
    public class PlayerSpawner
    {
        private readonly IPlayerFactory _factory;
        private readonly PlayerSpawnerConfig _config;

        public PlayerSpawner(IPlayerFactory factory, PlayerSpawnerConfig config)
        {
            _factory = factory;
            _config = config;
        }

        public Health Spawn() => 
            _factory.CreatePlayer(_config.Prefab, _config.SpawnPosition);
    }
}
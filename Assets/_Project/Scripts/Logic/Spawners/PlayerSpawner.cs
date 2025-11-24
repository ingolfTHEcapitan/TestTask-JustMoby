using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.Factory.PlayerFactory;

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
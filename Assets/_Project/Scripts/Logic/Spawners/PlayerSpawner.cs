using System.Threading.Tasks;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.ConfigsTemp;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Services.Factory.PlayerFactory;

namespace _Project.Scripts.Logic.Spawners
{
    public class PlayerSpawner
    {
        private readonly IPlayerFactory _factory;
        private readonly PlayerPrefabConfig _prefabConfig;
        private readonly PlayerSpawnerConfig _config;

        public PlayerSpawner(IPlayerFactory factory, PlayerPrefabConfig prefabConfig, PlayerSpawnerConfig config)
        {
            _factory = factory;
            _prefabConfig = prefabConfig;
            _config = config;
        }

        public async Task<Health> Spawn() => 
            await _factory.CreatePlayer(_prefabConfig.PrefabReference, _config.SpawnPosition);
    }
}
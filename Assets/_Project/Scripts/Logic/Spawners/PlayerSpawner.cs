using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player.Factory;
using Cysharp.Threading.Tasks;

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

        public async UniTask<Health> SpawnAsync() => 
            await _factory.CreatePlayerAsync(_config.SpawnPosition);
    }
}
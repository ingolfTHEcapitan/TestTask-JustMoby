using _Project._Scripts.Configs;
using _Project._Scripts.Infrastructure.Services.ConfigsManagement;
using _Project._Scripts.Infrastructure.Services.Factory;
using UnityEngine;

namespace _Project._Scripts.Logic
{
    public class PlayerSpawner
    {
        private readonly IGameFactory _factory;
        private readonly IConfigsProvider _configsProvider;

        public PlayerSpawner(IGameFactory factory, IConfigsProvider configsProvider)
        {
            _factory = factory;
            _configsProvider = configsProvider;
        }

        public GameObject Spawn(Transform parent)
        {
            PlayerSpawnerConfig config = _configsProvider.GetPlayerSpawner();
            return _factory.CreatePlayer(config.Prefab, config.SpawnPosition, parent);
        }
    }
}
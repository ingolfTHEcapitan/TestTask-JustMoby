using System.Collections.Generic;
using _Project._Scripts.Configs;

namespace _Project._Scripts.Infrastructure.Services.ConfigsManagement
{
    public interface IConfigsProvider
    {
        EnemySpawnerConfig EnemySpawner { get; }
        PlayerSpawnerConfig PlayerSpawner { get; }
        List<PlayerStatConfig> PlayerStats { get; }
    }
}
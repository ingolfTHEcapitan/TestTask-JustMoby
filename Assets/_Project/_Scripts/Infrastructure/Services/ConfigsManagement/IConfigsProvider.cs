using System.Collections.Generic;
using _Project._Scripts.Configs;

namespace _Project._Scripts.Infrastructure.Services.ConfigsManagement
{
    public interface IConfigsProvider
    {
        List<PlayerStatConfig> GetPlayerStats();
        EnemySpawnerConfig GetEnemySpawner();
        PlayerSpawnerConfig GetPlayerSpawner();
    }
}
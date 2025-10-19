using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Configs;
using UnityEngine;

namespace _Project._Scripts.Infrastructure.Services.ConfigsManagement
{
    public class ConfigsProvider : IConfigsProvider
    {
        private const string EnemySpawnerConfigPath = "Configs/Spawners/EnemySpawnerConfig";
        private const string PlayerSpawnerConfigPath = "Configs/Spawners/PlayerSpawnerConfig";
        private const string PlayerStatsPath = "Configs/PlayerStats";
        
        private List<PlayerStatConfig> _playerStats = new List<PlayerStatConfig>();
        private EnemySpawnerConfig _enemySpawnerConfig;
        private PlayerSpawnerConfig _playerSpawnerConfig;
        
        public void LoadEnemySpawner() => 
            _enemySpawnerConfig = Resources.Load<EnemySpawnerConfig>(EnemySpawnerConfigPath);
        
        public void LoadPlayerSpawner() => 
            _playerSpawnerConfig = Resources.Load<PlayerSpawnerConfig>(PlayerSpawnerConfigPath);

        public void LoadPlayerStats() => 
            _playerStats = Resources.LoadAll<PlayerStatConfig>(PlayerStatsPath).ToList();

        public List<PlayerStatConfig> GetPlayerStats() => 
            _playerStats;

        public EnemySpawnerConfig GetEnemySpawner() =>
            _enemySpawnerConfig;
        
        public PlayerSpawnerConfig GetPlayerSpawner() =>
            _playerSpawnerConfig;
    }
}
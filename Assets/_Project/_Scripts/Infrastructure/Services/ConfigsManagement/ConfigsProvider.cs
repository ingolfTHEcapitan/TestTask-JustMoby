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

        public EnemySpawnerConfig EnemySpawner { get; private set; }
        public PlayerSpawnerConfig PlayerSpawner { get; private set; }
        public List<PlayerStatConfig> PlayerStats { get; private set; } = new List<PlayerStatConfig>();

        public void Initialize()
        {
            LoadEnemySpawner();
            LoadPlayerStats();
            LoadPlayerSpawner();
        }
        
        private void LoadEnemySpawner() => 
            EnemySpawner = Resources.Load<EnemySpawnerConfig>(EnemySpawnerConfigPath);

        private void LoadPlayerSpawner() => 
            PlayerSpawner = Resources.Load<PlayerSpawnerConfig>(PlayerSpawnerConfigPath);

        private void LoadPlayerStats() => 
            PlayerStats = Resources.LoadAll<PlayerStatConfig>(PlayerStatsPath).ToList();
    }
}
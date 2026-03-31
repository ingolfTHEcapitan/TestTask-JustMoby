using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.Configs.IAP;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Services.RemoteConfig;
using Firebase.RemoteConfig;
using UnityEngine;

namespace _Project.Scripts.Services.Factory.RemoteConfigFactory
{
    public class RemoteConfigFactory : IRemoteConfigFactory
    {
        private readonly Dictionary<string, object> _configs;
        private readonly string[] _playerStatConfigKeys;
        private readonly IRemoteConfigService _remoteConfigService;
        private List<PlayerStatConfig> _playerStatConfigs;

        public RemoteConfigFactory(IRemoteConfigService remoteConfigService, PlayerSpawnerConfig playerSpawnerConfig, 
            EnemySpawnerConfig enemySpawnerConfig, WeaponConfig weaponConfig, BulletConfig bulletConfig, EnemyConfig enemyConfig, 
            SaveServiceConfig saveServiceConfig, ProductConfigWrapper productConfigWrapper, List<PlayerStatConfig> playerStatConfigs)
        {
            _playerStatConfigs = playerStatConfigs;
            _remoteConfigService = remoteConfigService;
            
            _configs = new Dictionary<string, object>()
            {
                {"player_spawner_config", playerSpawnerConfig},
                {"enemy_spawner_config", enemySpawnerConfig},
                {"weapon_config", weaponConfig},
                {"bullet_config",bulletConfig},
                {"enemy_skeleton_config", enemyConfig},
                {"save_service_config", saveServiceConfig},
                {"product_config", productConfigWrapper},
            };
            
            _playerStatConfigKeys = new[] { "stat_damage_config", "stat_health_config", "stat_speed_config" };
        }

        public void ApplyRemoteConfigs()
        { 
            FirebaseRemoteConfig remoteConfig = _remoteConfigService.RemoteConfig;

            LoadConfigs(remoteConfig);
            LoadPlayerStatConfigs(remoteConfig);
        }

        private void LoadConfigs(FirebaseRemoteConfig remoteConfigInstance)
        {
            foreach ((string firebaseKey, object configObject) in _configs)
            {
                string json = remoteConfigInstance.GetValue(firebaseKey).StringValue;
                JsonUtility.FromJsonOverwrite(json, configObject);
            }
        }

        private void LoadPlayerStatConfigs(FirebaseRemoteConfig remoteConfigInstance)
        {
            _playerStatConfigs.Clear();
            
            foreach (string configKey in _playerStatConfigKeys)
            {
                string json =  remoteConfigInstance.GetValue(configKey).StringValue;
                _playerStatConfigs.Add(JsonUtility.FromJson<PlayerStatConfig>(json));
            }
        }
    }
}
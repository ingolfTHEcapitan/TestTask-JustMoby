using System;
using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Services.RemoteConfig;
using Firebase.RemoteConfig;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.Factory.RemoteConfigFactory
{
    public class RemoteConfigFactory : IRemoteConfigFactory
    {
        private readonly DiContainer _container;
        private readonly Dictionary<string, Type> _configs;
        private readonly string[] _playerStatConfigKeys;
        private readonly IRemoteConfigService _remoteConfigService;

        public RemoteConfigFactory(IRemoteConfigService remoteConfigService, DiContainer container)
        {
            _remoteConfigService = remoteConfigService;
            _container = container;

            _configs = new Dictionary<string, Type>()
            {
                {"player_spawner_config", typeof(PlayerSpawnerConfig)},
                {"enemy_spawner_config", typeof(EnemySpawnerConfig)},
                {"weapon_config", typeof(WeaponConfig)},
                {"bullet_config", typeof(BulletConfig)},
                {"enemy_skeleton_config", typeof(EnemyConfig)},
            };
            
            _playerStatConfigKeys = new[] { "stat_damage_config", "stat_health_config", "stat_speed_config" };
        }

        public void ApplyRemoteConfigs()
        { 
            FirebaseRemoteConfig remoteConfig = _remoteConfigService.RemoteConfig;

            LoadAndBindConfigs(remoteConfig);
            LoadAndBindPlayerStatConfigs(remoteConfig);
        }

        private void LoadAndBindConfigs(FirebaseRemoteConfig remoteConfigInstance)
        {
            foreach ((string firebaseKey, Type configType) in _configs)
            {
                string json = remoteConfigInstance.GetValue(firebaseKey).StringValue;
                object config = JsonUtility.FromJson(json, configType);
                
                _container.Bind(configType).FromInstance(config).NonLazy();
            }
        }

        private void LoadAndBindPlayerStatConfigs(FirebaseRemoteConfig remoteConfigInstance)
        {
            List<PlayerStatConfig> playerStatConfigs = new List<PlayerStatConfig>();
            
            foreach (string configKey in _playerStatConfigKeys)
            {
                string json =  remoteConfigInstance.GetValue(configKey).StringValue;
                playerStatConfigs.Add(JsonUtility.FromJson<PlayerStatConfig>(json));
            }
            
            _container.Bind<List<PlayerStatConfig>>().FromInstance(playerStatConfigs).NonLazy();
        }
    }
}
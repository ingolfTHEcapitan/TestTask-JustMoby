using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.ConfigsTemp;
using _Project.Scripts.Services.RemoteConfig;
using Firebase.RemoteConfig;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.Factory.RemoteConfigFactory
{
    public class RemoteConfigFactory : IRemoteConfigFactory
    {
        private readonly DiContainer _container;
        private  FirebaseRemoteConfig _configInstance;
        
        private  Dictionary<string, object> _configs;
        private IRemoteConfigService _remoteConfigService;

        public RemoteConfigFactory(IRemoteConfigService remoteConfigService, DiContainer container)
        {
            _remoteConfigService = remoteConfigService;

            _container = container;

            /*_configs = new Dictionary<string, object>()
            {
                {"player_spawner_config", playerSpawnerConfig},
                {"enemy_spawner_config", enemySpawnerConfig},
                {"weapon_config", weaponConfig},
                {"bullet_config", bulletConfig},
                {"enemy_skeleton_config", enemyConfig},
            };

            foreach (PlayerStatUIConfig  playerStatConfig in playerStatConfigs) 
                _configs.Add($"stat_{playerStatConfig.name.ToLower()}_config", playerStatConfig);*/
        }

        public void ApplyRemoteSettings()
        {
            _configInstance = _remoteConfigService.RemoteConfigInstance;
            /*foreach ((string firebaseKey, ScriptableObject configInstance) in _configs)
            {
                string configValues = _configInstance.GetValue(firebaseKey).StringValue;
                JsonUtility.FromJsonOverwrite(configValues, configInstance);
                
                Debug.Log($"RemoteConfig: Successfully updated {configInstance.name} from key {firebaseKey}");
            }*/
            
            string enemySpawnerJson= _configInstance.GetValue("enemy_spawner_config").StringValue;
            string playerSpawnerJson= _configInstance.GetValue("player_spawner_config").StringValue;
            string bulletConfigJson= _configInstance.GetValue("bullet_config").StringValue;
            string weaponConfigJson= _configInstance.GetValue("weapon_config").StringValue;
            string enemySkeletonJson= _configInstance.GetValue("enemy_skeleton_config").StringValue;
            string statDamageJson= _configInstance.GetValue("stat_damage_config").StringValue;
            string statHealthJson= _configInstance.GetValue("stat_health_config").StringValue;
            string statSpeedJson= _configInstance.GetValue("stat_speed_config").StringValue;

            EnemySpawnerConfig enemySpawnerConfig = JsonUtility.FromJson<EnemySpawnerConfig>(enemySpawnerJson);
            PlayerSpawnerConfig playerSpawnerConfig = JsonUtility.FromJson<PlayerSpawnerConfig>(playerSpawnerJson);
            BulletConfig bulletConfig = JsonUtility.FromJson<BulletConfig>(bulletConfigJson);
            WeaponConfig weaponConfig = JsonUtility.FromJson<WeaponConfig>(weaponConfigJson);
            EnemyConfig enemyConfig = JsonUtility.FromJson<EnemyConfig>(enemySkeletonJson);
            
           
            
            List<PlayerStatConfig> playerStatConfigs = new List<PlayerStatConfig>()
            {
                JsonUtility.FromJson<PlayerStatConfig>(statDamageJson),
                JsonUtility.FromJson<PlayerStatConfig>(statHealthJson),
                JsonUtility.FromJson<PlayerStatConfig>(statSpeedJson),
            };

            _container.Bind<List<PlayerStatConfig>>().FromInstance(playerStatConfigs).NonLazy();
            
            _container.Bind<EnemySpawnerConfig>().FromInstance(enemySpawnerConfig).NonLazy();
            _container.Bind<PlayerSpawnerConfig>().FromInstance(playerSpawnerConfig).NonLazy();
            _container.Bind<BulletConfig>().FromInstance(bulletConfig).NonLazy();
            _container.Bind<WeaponConfig>().FromInstance(weaponConfig).NonLazy();
            _container.Bind<EnemyConfig>().FromInstance(enemyConfig).NonLazy();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Configs.Weapon;
using Firebase.RemoteConfig;
using UnityEngine;

namespace _Project.Scripts.Configs 
{
    public class RemoteConfigService
    {
        private readonly Dictionary<string, ScriptableObject> _firebaseRemoteConfigs;

        public RemoteConfigService(List<PlayerStatConfig> playerStatConfigs, PlayerSpawnerConfig playerSpawnerConfig, 
            EnemySpawnerConfig enemySpawnerConfig, WeaponConfig weaponConfig, BulletConfig bulletConfig, EnemyConfig enemyConfig)
        {
            
            _firebaseRemoteConfigs = new Dictionary<string, ScriptableObject>()
            {
                {"stat_damage_config", playerStatConfigs[0]},
                {"stat_health_config", playerStatConfigs[1]},
                {"stat_speed_config", playerStatConfigs[2]},
                
                {"player_spawner_config", playerSpawnerConfig},
                {"enemy_spawner_config", enemySpawnerConfig},
                {"weapon_config", weaponConfig},
                {"bullet_config", bulletConfig},
                {"enemy_skeleton_config", enemyConfig},
                
            };
        }
        
        public async Task FetchDataAsync()
        {
            Debug.Log("Fetching data...");

            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
            await fetchTask;
            
            if (!fetchTask.IsCompleted)
            {
                Debug.LogError("Retrieval hasn't finished.");
                return;
            }
            
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            ConfigInfo info = remoteConfig.Info;
            if (info.LastFetchStatus != LastFetchStatus.Success)
            {
                Debug.LogError($"fetchComplete was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
                return;
            }

            await remoteConfig.ActivateAsync();
            
            Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
            ApplyRemoteSettings(remoteConfig);
        }
        
        private void ApplyRemoteSettings(FirebaseRemoteConfig remoteConfig)
        {
            foreach ((string firebaseKey, ScriptableObject configInstance) in _firebaseRemoteConfigs)
            {
                ConfigValue configValue = remoteConfig.GetValue(firebaseKey);

                if (configValue.Source != ValueSource.RemoteValue && string.IsNullOrEmpty(configValue.StringValue))
                {
                    Debug.LogWarning($"RemoteConfig: No remote value for key '{firebaseKey}', keeping local defaults.");
                    continue;
                }
                
                string json = configValue.StringValue;
                
                JsonUtility.FromJsonOverwrite(json, configInstance);
                Debug.Log($"RemoteConfig: Successfully updated {configInstance.name} from key {firebaseKey}");
            }
        }
    }
}
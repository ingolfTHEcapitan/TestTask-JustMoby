using System.Collections.Generic;
using System.IO;
using _Project.Scripts.Configs;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.ConfigsTemp;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.Ads;
using _Project.Scripts.Services.Analytics;
using _Project.Scripts.Services.Factory.BulletFactory;
using _Project.Scripts.Services.Factory.EnemyFactory;
using _Project.Scripts.Services.Factory.PlayerFactory;
using _Project.Scripts.Services.Factory.RemoteConfigFactory;
using _Project.Scripts.Services.Factory.UIFactory;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.HealthCalculator;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.RemoteConfig;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.Services.Score;
using _Project.Scripts.Services.Statistics;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameInstaller: MonoInstaller
    {
        [Header("Transforms")]
        [SerializeField] private Transform _dynamicObjectsParent;
        [SerializeField] private Transform _uiParent;
        [SerializeField] private Transform _gameParent;
        [SerializeField] private Transform _enemySpawnPoint;
        
        [Header("Configs")]
        [SerializeField] private List<PlayerStatUIConfig> _playerStatUIConfigs;
        [SerializeField] private PlayerPrefabConfig playerPrefabConfig;
        [SerializeField] private EnemyPrefabConfig enemyPrefabConfig;
        [SerializeField] private BulletPrefabConfig bulletPrefabConfig;
        [SerializeField] private SaveServiceConfig _saveServiceConfig;
        public override void InstallBindings()
        {
            BindConfigs();
            BindServices();
            BindPlayer();
            BindPlayerStats();
            BindScoreService();
            BindEnemy();
            BindWeapon();
            BindGameBootstrapper();
        }

        private void BindConfigs()
        {
            Container.Bind<List<PlayerStatUIConfig>>().FromInstance(_playerStatUIConfigs).AsSingle();
            Container.Bind<PlayerPrefabConfig>().FromInstance(playerPrefabConfig).AsSingle();
            Container.Bind<EnemyPrefabConfig>().FromInstance(enemyPrefabConfig).AsSingle();
            Container.Bind<BulletPrefabConfig>().FromInstance(bulletPrefabConfig).AsSingle();
        }
        
        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<UIFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ISaveLoadService>().FromInstance(_saveServiceConfig.GetInstance()).AsSingle();
            Container.BindInterfacesAndSelfTo<HealthCalculatorService>().AsSingle();
            Container.BindInterfacesAndSelfTo<FirebaseAnalyticsService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStatistics>().AsSingle();
        }

        private void BindPlayer()
        {
            Container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().WithArguments(_gameParent);
            Container.Bind<PlayerSpawner>().AsSingle();
        }

        private void BindPlayerStats() => 
            Container.BindInterfacesAndSelfTo<PlayerStatsModel>().AsSingle();

        private void BindScoreService() => 
            Container.BindInterfacesAndSelfTo<ScoreService>().AsSingle();

        private void BindEnemy()
        {
            Container.BindInterfacesAndSelfTo<EnemyFactory>().AsSingle().WithArguments(_dynamicObjectsParent);
            Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle();
        }

        private void BindWeapon() => 
            Container.BindInterfacesAndSelfTo<BulletFactory>().AsSingle().WithArguments(_dynamicObjectsParent);

        private void BindGameBootstrapper()
        {
            Container.BindInterfacesAndSelfTo<GameBootstrapper>().AsSingle()
                .WithArguments(_enemySpawnPoint, _uiParent).NonLazy();
        }
    }
}
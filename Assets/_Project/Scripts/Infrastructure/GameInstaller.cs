using System.Collections.Generic;
using _Project.Scripts.Configs;
using _Project.Scripts.Configs.Spawners;
using _Project.Scripts.Configs.Weapon;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.Factory.BulletFactory;
using _Project.Scripts.Services.Factory.EnemyFactory;
using _Project.Scripts.Services.Factory.PlayerFactory;
using _Project.Scripts.Services.Factory.UIFactory;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.HealthCalculator;
using _Project.Scripts.Services.PlayerInput;
using _Project.Scripts.Services.SaveLoad;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts.Infrastructure
{
    public class GameInstaller: MonoInstaller
    {
        [Header("Transforms")]
        [SerializeField] private Transform _dynamicObjectsParent;
        [SerializeField] private Transform _uiParent;
        [SerializeField] private Transform _gameParent;
        [SerializeField] private Transform _enemySpawnPoint;
        [Header("Configs")]
        [SerializeField] private List<PlayerStatConfig> _playerStatConfigs;
        [SerializeField] private PlayerSpawnerConfig _playerSpawnerConfig;
        [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;
        [SerializeField] private WeaponConfig _weaponConfig;
        [SerializeField] private EnemyConfig _enemyConfig;
        [SerializeField] private SaveServiceConfig _saveServiceConfig;
        [Header("Prefabs")]
        [SerializeField] private GameObject _hudPrefab;
        [SerializeField] private GameObject _popUpLayerPrefab;
        
        public override void InstallBindings()
        {
            BindServices();
            BindPlayer();
            BindPlayerStats();
            BindEnemy();
            BindWeapon();
            BindGameBootstrapper();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<ISaveLoadService>().FromInstance(_saveServiceConfig.GetInstance()).AsSingle();
            Container.BindInterfacesAndSelfTo<DesktopInputService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GamePauseService>().AsSingle();
            Container.BindInterfacesAndSelfTo<HealthCalculatorService>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIFactory>().AsSingle().WithArguments(_uiParent);
        }

        private void BindPlayer()
        {
            Container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().WithArguments(_gameParent);
            Container.Bind<PlayerSpawnerConfig>().FromInstance(_playerSpawnerConfig).AsSingle();
            Container.Bind<PlayerSpawner>().AsSingle();
        }

        private void BindPlayerStats()
        {
            Container.Bind<List<PlayerStatConfig>>().FromInstance(_playerStatConfigs).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerStatsModel>().AsSingle();
        }

        private void BindEnemy()
        {
            Container.BindInterfacesAndSelfTo<EnemyFactory>().AsSingle().WithArguments(_dynamicObjectsParent);
            Container.Bind<EnemySpawnerConfig>().FromInstance(_enemySpawnerConfig).AsSingle();
            Container.Bind<EnemyConfig>().FromInstance(_enemyConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle();
        }

        private void BindWeapon()
        {
            Container.BindInterfacesAndSelfTo<BulletFactory>().AsSingle().WithArguments(_dynamicObjectsParent);
            Container.Bind<WeaponConfig>().FromInstance(_weaponConfig).AsSingle();
        }

        private void BindGameBootstrapper()
        {
            Container.BindInterfacesAndSelfTo<GameBootstrapper>().AsSingle()
                .WithArguments(_hudPrefab, _popUpLayerPrefab, _enemySpawnPoint).NonLazy();
        }
    }
}
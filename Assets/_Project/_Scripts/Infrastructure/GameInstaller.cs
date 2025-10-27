using System.Collections.Generic;
using _Project._Scripts.Configs;
using _Project._Scripts.Configs.Weapon;
using _Project._Scripts.Infrastructure.Services.Factory;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.HealthCalculator;
using _Project._Scripts.Infrastructure.Services.PlayerInput;
using _Project._Scripts.Infrastructure.Services.SaveLoad;
using _Project._Scripts.Logic.PlayerStats;
using _Project._Scripts.Logic.Spawners;
using UnityEngine;
using Zenject;

namespace _Project._Scripts.Infrastructure
{
    public class GameInstaller: MonoInstaller
    {
        [Header("Transforms")]
        [SerializeField] private Transform _dynamicObjectsParent;
        [SerializeField] private Transform _UIParent;
        [SerializeField] private Transform _gameParent;
        [Header("Configs")]
        [SerializeField] private List<PlayerStatConfig> _playerStatConfigs;
        [SerializeField] private PlayerSpawnerConfig _playerSpawnerConfig;
        [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;
        [SerializeField] private WeaponConfig _weaponConfig;
        
        public override void InstallBindings()
        {
            
            
            BindServices();
            BindPlayer();
            BindPlayerStats();
            BindEnemy();

            Container.Bind<WeaponConfig>().FromInstance(_weaponConfig).AsSingle();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<DesktopInputService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GamePauseService>().AsSingle();
            Container.BindInterfacesAndSelfTo<SaveLoadService>().AsSingle();
            Container.BindInterfacesAndSelfTo<HealthCalculatorService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameFactory>().AsSingle()
                .WithArguments(_dynamicObjectsParent, _UIParent, _gameParent);
        }

        private void BindPlayer()
        {
            Container.Bind<PlayerSpawnerConfig>().FromInstance(_playerSpawnerConfig).AsSingle();
            Container.Bind<PlayerSpawner>().AsSingle();
        }

        private void BindPlayerStats()
        {
            Container.Bind<List<PlayerStatConfig>>().FromInstance(_playerStatConfigs).AsSingle();
            Container.Bind<PlayerStatsModel>().AsSingle();
        }

        private void BindEnemy()
        {
            Container.Bind<EnemySpawnerConfig>().FromInstance(_enemySpawnerConfig).AsSingle();
            Container.Bind<EnemySpawner>().AsSingle();
        }
    }
}
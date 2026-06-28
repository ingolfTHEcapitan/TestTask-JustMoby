using _Project.Scripts.Logic.Enemy.Factory;
using _Project.Scripts.Logic.Player.Factory;
using _Project.Scripts.Logic.Player.PlayerStats;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using _Project.Scripts.Logic.Player.Weapon.Bullet.Factory;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.HealthCalculator;
using _Project.Scripts.Services.UpgradePoints;
using UnityEngine;
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
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _dungeonMusic;
        
        public override void InstallBindings()
        {
            BindPlayer();
            BindPlayerStats();
            BindHealthCalculatorService();
            BindUpgradePointsService();
            BindEnemy();
            BindWeapon();
            BindGameBootstrapper();
        }
        
        private void BindHealthCalculatorService() => 
            Container.BindInterfacesAndSelfTo<HealthCalculatorService>().AsSingle();

        private void BindPlayer()
        {
            Container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().WithArguments(_gameParent);
            Container.Bind<PlayerSpawner>().AsSingle();
        }

        private void BindPlayerStats()
        {
            Container.BindInterfacesAndSelfTo<PlayerStatsData>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerStatsSaveLoad>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerStatsModel>().AsSingle();
        }

        private void BindUpgradePointsService() => 
            Container.BindInterfacesAndSelfTo<UpgradePointsService>().AsSingle();

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
                .WithArguments(_enemySpawnPoint, _uiParent, _audioSource, _dungeonMusic);
        }
    }
}
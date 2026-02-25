using _Project.Scripts.Configs;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.Factory.BulletFactory;
using _Project.Scripts.Services.Factory.EnemyFactory;
using _Project.Scripts.Services.Factory.PlayerFactory;
using _Project.Scripts.Services.Factory.UIFactory;
using _Project.Scripts.Services.HealthCalculator;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.Services.Score;
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

        private SaveServiceConfig _saveServiceConfig;
        
        [Inject] 
        private void Construct(SaveServiceConfig saveServiceConfig) => 
            _saveServiceConfig = saveServiceConfig;

        public override void InstallBindings()
        {
            BindServices();
            BindPlayer();
            BindPlayerStats();
            BindScoreService();
            BindEnemy();
            BindWeapon();
            BindGameBootstrapper();
        }
        
        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<UIFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ISaveLoadService>().FromInstance(_saveServiceConfig.GetInstance()).AsSingle();
            Container.BindInterfacesAndSelfTo<HealthCalculatorService>().AsSingle();
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
using _Project.Scripts.Logic;
using _Project.Scripts.Logic.Enemy;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Player.Weapon.Bullet;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Tools;
using _Project.Scripts.UI.Common.HealthBar;
using _Project.Scripts.UI.HUD;
using _Project.Scripts.UI.Windows.GameOver;
using _Project.Scripts.UI.Windows.PlayerStats;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Transforms")]
        [SerializeField] private Transform _dynamicObjectsParent;
        [SerializeField] private Transform _uiParent;
        [SerializeField] private Transform _gameParent;
        [SerializeField] private Transform _enemySpawnPoint;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioClip _gameplayMusic;
        
        public override void InstallBindings()
        {
            BindServices();
            BindPlayer();
            BindPlayerStats();
            BindHeadUpDisplay();
            BindHealthBarFactory();
            BindGameOverWindow();
            BindHealthCalculator();
            BindUpgradePointsService();
            BindEnemy();
            BindWeapon();
            BindGame();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<GamePauseService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStatistics>().AsSingle();
            Container.BindInterfacesAndSelfTo<EffectsService>().AsSingle();
        }

        private void BindHealthCalculator() => 
            Container.Bind<HealthCalculator>().AsSingle();

        private void BindHealthBarFactory() => 
            Container.Bind<HealthBarFactory>().AsSingle();

        private void BindPlayer()
        {
            Container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().WithArguments(_gameParent);
            Container.Bind<PlayerSpawner>().AsSingle();
            Container.Bind<PlayerContainer>().FromMethod(GetPlayer).AsSingle();
        }

        private void BindPlayerStats()
        {
            Container.BindInterfacesAndSelfTo<PlayerStatItemFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerStatsModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerStatsSaveLoad>().AsSingle();
            Container.Bind<PlayerStatsWindowModel>().AsSingle();
            Container.BindWindow<PlayerStatsWindowView>();
            Container.Bind<PlayerStatsWindowPresenter>().AsSingle();
        }
        
        private void BindHeadUpDisplay()
        {
            Container.BindWindow<HeadUpDisplayView>();
            Container.BindInterfacesAndSelfTo<HeadUpDisplayModel>().AsSingle();
            Container.Bind<HeadUpDisplayPresenter>().AsSingle();
        }

        private void BindGameOverWindow()
        {
            Container.BindWindow<GameOverWindowView>();
            Container.Bind<GameOverWindowModel>().AsSingle();
            Container.Bind<GameOverWindowPresenter>().AsSingle();
        }

        private void BindUpgradePointsService() => 
            Container.BindInterfacesAndSelfTo<ScoreService>().AsSingle();

        private void BindEnemy()
        {
            Container.BindInterfacesAndSelfTo<EnemyFactory>().AsSingle().WithArguments(_dynamicObjectsParent);
            Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle();
        }

        private void BindWeapon() => 
            Container.BindInterfacesAndSelfTo<BulletFactory>().AsSingle().WithArguments(_dynamicObjectsParent);

        private void BindGame()
        {
            Container.Bind<GameUIInitializer>().AsSingle().WithArguments(_uiParent);
            Container.BindInterfacesAndSelfTo<GameBootstrapper>().AsSingle().WithArguments(_enemySpawnPoint,  _musicSource, _gameplayMusic);
        }
        
        private PlayerContainer GetPlayer(InjectContext context) => 
            context.Container.Resolve<PlayerFactory>().GetPlayer();
    }
}
using _Project.Scripts.Logic.Enemy.Factory;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Player.Factory;
using _Project.Scripts.Logic.Player.Weapon.Bullet.Factory;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.HealthCalculator;
using _Project.Scripts.Services.UpgradePoints;
using _Project.Scripts.UI.Factory;
using _Project.Scripts.UI.HUD;
using _Project.Scripts.UI.Windows.GameOver;
using _Project.Scripts.UI.Windows.PlayerStats;
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
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioClip _dungeonMusic;
        [SerializeField] private AudioClip _levelUpSounds;
        
        public override void InstallBindings()
        {
            BindPlayer();
            BindPlayerStats();
            BindHeadUpDisplay();
            BindGameOverWindow();
            BindHealthCalculatorService();
            BindUpgradePointsService();
            BindEnemy();
            BindWeapon();
            BindGame();
        }
        
        private void BindHealthCalculatorService() => 
            Container.BindInterfacesAndSelfTo<HealthCalculatorService>().AsSingle();

        private void BindPlayer()
        {
            Container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().WithArguments(_gameParent);
            Container.Bind<PlayerSpawner>().AsSingle();
            Container.Bind<PlayerDeath>().FromMethod(GetPlayerDeath).AsSingle();
        }

        private void BindPlayerStats()
        {
            Container.BindInterfacesAndSelfTo<PlayerStatsModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerStatsSaveLoad>().AsSingle();
            Container.Bind<PlayerStatsWindowModel>().AsSingle();
            Container.Bind<PlayerStatsWindowView>().FromMethod(GetPlayerStatsWindowView).AsSingle();
            Container.Bind<PlayerStatsWindowPresenter>().AsSingle();
        }
        
        private void BindHeadUpDisplay()
        {
            Container.Bind<HeadUpDisplayView>().FromMethod(GetHudView).AsSingle();
            Container.BindInterfacesAndSelfTo<HeadUpDisplayModel>().AsSingle();
            Container.Bind<HeadUpDisplayPresenter>().AsSingle();
        }

        private void BindGameOverWindow()
        {
            Container.Bind<GameOverWindowView>().FromMethod(GetGameOverView).AsSingle();
            Container.Bind<GameOverWindowModel>().AsSingle();
            Container.Bind<GameOverWindowPresenter>().AsSingle();
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

        private void BindGame()
        {
            Container.Bind<GameUIInitializer>().AsSingle().WithArguments(_uiParent, _levelUpSounds);
            Container.Bind<GameStarter>().AsSingle().WithArguments( _musicSource, _dungeonMusic);
            Container.BindInterfacesAndSelfTo<GameBootstrapper>().AsSingle().WithArguments(_enemySpawnPoint);
        }

        private HeadUpDisplayView GetHudView(InjectContext context) =>
            context.Container.Resolve<IUIFactory>().GetHudView();

        private GameOverWindowView GetGameOverView(InjectContext context) => 
            context.Container.Resolve<IUIFactory>().GetGameOverWindowView();

        private PlayerDeath GetPlayerDeath(InjectContext context) => 
            context.Container.Resolve<IPlayerFactory>().GetPlayerDeath();
        
        private PlayerStatsWindowView GetPlayerStatsWindowView(InjectContext context)=> 
            context.Container.Resolve<IUIFactory>().GetPlayerStatsWindowView();
    }
}
using System;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.UpgradePoints;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using _Project.Scripts.UI.HUD;
using _Project.Scripts.UI.Windows.GameOver;
using _Project.Scripts.UI.Windows.PlayerStats;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameUIInitializer: IDisposable
    {
        private readonly IUIFactory _uiFactory;
        private readonly Transform _uiParent;
       
        private readonly PlayerStatsWindowPresenter _playerStatsWindowPresenter;
        private readonly EnemySpawner _enemySpawner;
        private readonly AudioClip _levelUpSound;
        private readonly IUpgradePointsService _pointsService;
        private readonly LazyInject<HeadUpDisplayPresenter> _lazyHudPresenter;
        private HeadUpDisplayPresenter _hudPresenter;

        public GameUIInitializer(IUIFactory uiFactory, Transform uiParent, AudioClip levelUpSound, PlayerStatsWindowPresenter playerStatsWindowPresenter, 
            EnemySpawner enemySpawner, IUpgradePointsService pointsService, LazyInject<HeadUpDisplayPresenter> lazyHudPresenter)
        {
            _pointsService = pointsService;
            _uiFactory = uiFactory;
            _uiParent = uiParent;
            _playerStatsWindowPresenter = playerStatsWindowPresenter;
            _enemySpawner = enemySpawner;
            _levelUpSound = levelUpSound;
            _lazyHudPresenter = lazyHudPresenter;
        }

        public void Dispose() => 
            _hudPresenter.Dispose();

        public async UniTask InitUIAsync(Health playerHealth)
        {
            HeadUpDisplayView hudView = await _uiFactory.CreateHudViewAsync(_uiParent);
            

            InitPlayerHealthBarView(hudView, playerHealth);
            _hudPresenter = _lazyHudPresenter.Value;
            _hudPresenter.Initialize();
            
            PlayerStatsWindowView playerStatsWindowView = await InitPlayerStatsView(hudView, _levelUpSound, _pointsService);
            await InitPlayerStatsPresenterAsync(playerStatsWindowView, playerHealth);
            
            await InitGameOverWindow(playerHealth, _enemySpawner);
        }

        private void InitPlayerHealthBarView(HeadUpDisplayView hud, Health playerHealth)
        {
            HealthBarView playerHealthBarView = hud.HealthBarView;
            playerHealthBarView.Construct(playerHealth);
            playerHealthBarView.Initialize();
        }

        private async UniTask<PlayerStatsWindowView> InitPlayerStatsView(HeadUpDisplayView hud, AudioClip levelUpSound, IUpgradePointsService pointsService)
        {
            Button openButton = hud.OpenStatsWindowButton;
            
            PlayerStatsWindowView playerStatsWindowView = await _uiFactory.CreatePlayerStatsViewAsync(_uiParent);
            playerStatsWindowView.Initialize(openButton, levelUpSound, pointsService);
            return playerStatsWindowView;
        }

        private async UniTask InitPlayerStatsPresenterAsync(PlayerStatsWindowView view, Health player)
        {
            PlayerDeath playerDeath = player.GetComponent<PlayerDeath>();
            _playerStatsWindowPresenter.Construct(view, playerDeath);
            await _playerStatsWindowPresenter.InitializeAsync();
        }

        private async UniTask InitGameOverWindow(Health player, EnemySpawner enemySpawner)
        {
            PlayerDeath playerDeath = player.GetComponent<PlayerDeath>();
            GameOverWindow gameOverWindow = await _uiFactory.CreateGameOverWindowAsync(_uiParent);
            gameOverWindow.Initialize(playerDeath, enemySpawner);
        }
    }
}
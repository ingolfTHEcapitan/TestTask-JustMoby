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
        private readonly AudioClip _levelUpSound;
        private readonly IUpgradePointsService _pointsService;
        private readonly LazyInject<HeadUpDisplayPresenter> _lazyHudPresenter;
        private readonly LazyInject<GameOverWindowPresenter> _lazyGameOverWindowPresenter;
        private HeadUpDisplayPresenter _hudPresenter;
        private GameOverWindowPresenter _gameOverWindowPresenter;

        public GameUIInitializer(IUIFactory uiFactory, Transform uiParent, AudioClip levelUpSound, PlayerStatsWindowPresenter playerStatsWindowPresenter, IUpgradePointsService pointsService, LazyInject<HeadUpDisplayPresenter> lazyHudPresenter,
            LazyInject<GameOverWindowPresenter> lazyGameOverWindowPresenter)
        {
            _pointsService = pointsService;
            _uiFactory = uiFactory;
            _uiParent = uiParent;
            _playerStatsWindowPresenter = playerStatsWindowPresenter;
            _levelUpSound = levelUpSound;
            _lazyHudPresenter = lazyHudPresenter;
            _lazyGameOverWindowPresenter = lazyGameOverWindowPresenter;
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
            
            GameOverWindowView gameOverWindowView = await _uiFactory.CreateGameOverWindowViewAsync(_uiParent);
            _gameOverWindowPresenter = _lazyGameOverWindowPresenter.Value;
            _gameOverWindowPresenter.Initialize();
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
    }
}
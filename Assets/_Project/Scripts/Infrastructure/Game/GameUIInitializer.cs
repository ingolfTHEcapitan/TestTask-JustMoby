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

        private readonly AudioClip _levelUpSound;
        private readonly LazyInject<HeadUpDisplayPresenter> _lazyHudPresenter;
        private readonly LazyInject<GameOverWindowPresenter> _lazyGameOverWindowPresenter;
        private readonly LazyInject<PlayerStatsWindowPresenter> _lazyPlayerStatsWindowPresenter;
        private HeadUpDisplayPresenter _hudPresenter;
        private GameOverWindowPresenter _gameOverWindowPresenter;
        private PlayerStatsWindowPresenter _playerStatsWindowPresenter;

        public GameUIInitializer(IUIFactory uiFactory, Transform uiParent, AudioClip levelUpSound, 
            LazyInject<PlayerStatsWindowPresenter> lazyPlayerStatsWindowPresenter, LazyInject<HeadUpDisplayPresenter> lazyHudPresenter,
            LazyInject<GameOverWindowPresenter> lazyGameOverWindowPresenter)
        {
            _uiFactory = uiFactory;
            _uiParent = uiParent;
            _levelUpSound = levelUpSound;
            _lazyPlayerStatsWindowPresenter = lazyPlayerStatsWindowPresenter;
            _lazyGameOverWindowPresenter = lazyGameOverWindowPresenter;
            _lazyHudPresenter = lazyHudPresenter;
        }

        public void Dispose()
        {
            _hudPresenter.Dispose();
            _playerStatsWindowPresenter.Dispose();
            _gameOverWindowPresenter.Dispose();
        }

        public async UniTask InitUIAsync(Health playerHealth)
        {
            HeadUpDisplayView hudView = await _uiFactory.CreateHudViewAsync(_uiParent);
            InitPlayerHealthBarView(hudView, playerHealth);
            _hudPresenter = _lazyHudPresenter.Value;
            _hudPresenter.Initialize();
            
            PlayerStatsWindowView playerStatsWindowView = await InitPlayerStatsView(hudView, _levelUpSound);
            _playerStatsWindowPresenter = _lazyPlayerStatsWindowPresenter.Value;
            await _playerStatsWindowPresenter.InitializeAsync();
            
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

        private async UniTask<PlayerStatsWindowView> InitPlayerStatsView(HeadUpDisplayView hud, AudioClip levelUpSound)
        {
            Button openButton = hud.OpenStatsWindowButton;
            
            PlayerStatsWindowView playerStatsWindowView = await _uiFactory.CreatePlayerStatsViewAsync(_uiParent);
            playerStatsWindowView.Initialize(openButton, levelUpSound);
            return playerStatsWindowView;
        }
    }
}
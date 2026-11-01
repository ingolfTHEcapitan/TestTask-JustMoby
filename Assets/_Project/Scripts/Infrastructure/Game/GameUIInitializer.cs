using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Common.HealthBar;
using _Project.Scripts.UI.HUD;
using _Project.Scripts.UI.Windows.GameOver;
using _Project.Scripts.UI.Windows.PlayerStats;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameUIInitializer
    {
        private readonly IInstantiator _gameContainer;
        private readonly WindowFactory _windowFactory;
        private readonly Transform _uiParent;
        private readonly HealthBarFactory _healthBarFactory;
        private readonly LazyInject<HeadUpDisplayPresenter> _lazyHudPresenter;
        private readonly LazyInject<GameOverWindowPresenter> _lazyGameOverWindowPresenter;
        private readonly LazyInject<PlayerStatsWindowPresenter> _lazyPlayerStatsWindowPresenter;
        private readonly LazyInject<PlayerContainer> _lazyPlayer;

        public GameUIInitializer(IInstantiator gameContainer, WindowFactory windowFactory, Transform uiParent, HealthBarFactory healthBarFactory,
            LazyInject<PlayerStatsWindowPresenter> lazyPlayerStatsWindowPresenter, LazyInject<HeadUpDisplayPresenter> lazyHudPresenter,
            LazyInject<GameOverWindowPresenter> lazyGameOverWindowPresenter, LazyInject<PlayerContainer> lazyPlayer)
        {
            _gameContainer = gameContainer;
            _windowFactory = windowFactory;
            _uiParent = uiParent;
            _lazyPlayerStatsWindowPresenter = lazyPlayerStatsWindowPresenter;
            _lazyGameOverWindowPresenter = lazyGameOverWindowPresenter;
            _lazyHudPresenter = lazyHudPresenter;
            _healthBarFactory = healthBarFactory;
            _lazyPlayer = lazyPlayer;
        }
        
        public async UniTask InitUIAsync()
        {
            await _windowFactory.CreateAsync<PlayerStatsWindowView>(_uiParent, sceneContainer: _gameContainer);
            await _lazyPlayerStatsWindowPresenter.Value.InitializeAsync();
            
            HeadUpDisplayView hudView = await _windowFactory.CreateAsync<HeadUpDisplayView>(_uiParent);
            _lazyHudPresenter.Value.Initialize();

            CreatePlayerHealthBar(hudView, _lazyPlayer.Value.Health);

            await _windowFactory.CreateAsync<GameOverWindowView>(_uiParent);
            _lazyGameOverWindowPresenter.Value.Initialize();
        }

        private void CreatePlayerHealthBar(HeadUpDisplayView hud, Health playerHealthModel) => 
            _healthBarFactory.Create(hud.HealthBarView, playerHealthModel, hideOnZeroHealth: false);
    }
}
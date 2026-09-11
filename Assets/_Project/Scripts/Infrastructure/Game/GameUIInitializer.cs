using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.Player.PlayerStats;
using _Project.Scripts.Logic.Player.PlayerStats.UI;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.UpgradePoints;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Common;
using _Project.Scripts.UI.Factory;
using _Project.Scripts.UI.HUD;
using _Project.Scripts.UI.Windows.GameOver;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameUIInitializer
    {
        private readonly IUIFactory _uiFactory;
        private readonly Transform _uiParent;
       
        private readonly PlayerStatsPresenter _playerStatsPresenter;
        private readonly EnemySpawner _enemySpawner;
        private readonly AudioClip _levelUpSound;
        private readonly IUpgradePointsService _pointsService;
        private HeadUpDisplayPresenter _hudPresenter;

        public GameUIInitializer(IUIFactory uiFactory, Transform uiParent, AudioClip levelUpSound, PlayerStatsPresenter playerStatsPresenter, 
            EnemySpawner enemySpawner, IUpgradePointsService pointsService, HeadUpDisplayPresenter hudPresenter)
        {
            _pointsService = pointsService;
            _uiFactory = uiFactory;
            _uiParent = uiParent;
            _playerStatsPresenter = playerStatsPresenter;
            _enemySpawner = enemySpawner;
            _levelUpSound = levelUpSound;
            _hudPresenter = hudPresenter;
        }

        public async UniTask InitUIAsync(Health playerHealth)
        {
            HeadUpDisplayView hudView = await _uiFactory.CreateHudLayerAsync(_uiParent);
            GameObject popUpLayer = await _uiFactory.CreatePopUpLayerAsync(_uiParent);

            InitPlayerHealthBarView(hudView, playerHealth);
            InitHudPresenter(hudView);
            
            PlayerStatsView playerStatsView = InitPlayerStatsView(popUpLayer, hudView, _levelUpSound, _pointsService);
            await InitPlayerStatsPresenterAsync(playerStatsView, playerHealth);
            
            InitGameOverWindow(popUpLayer, playerHealth, _enemySpawner);
        }

        private void InitHudPresenter(HeadUpDisplayView hudView)
        {
            _hudPresenter.Construct(hudView);
            _hudPresenter.Initialize();
        }

        private void InitPlayerHealthBarView(HeadUpDisplayView hud, Health playerHealth)
        {
            HealthBarView playerHealthBarView = hud.HealthBarView;
            playerHealthBarView.Construct(playerHealth);
            playerHealthBarView.Initialize();
        }

        private PlayerStatsView InitPlayerStatsView(GameObject popUpLayer, HeadUpDisplayView hud, AudioClip levelUpSound,
            IUpgradePointsService pointsService)
        {
            Button openButton = hud.OpenStatsWindowButton;
            
            PlayerStatsView playerStatsView = popUpLayer.GetComponentInChildren<PlayerStatsView>(includeInactive: true);
            playerStatsView.Initialize(openButton, levelUpSound, pointsService);
            return playerStatsView;
        }

        private async UniTask InitPlayerStatsPresenterAsync(PlayerStatsView view, Health player)
        {
            PlayerDeath playerDeath = player.GetComponent<PlayerDeath>();
            _playerStatsPresenter.Construct(view, playerDeath);
            await _playerStatsPresenter.InitializeAsync();
        }

        private void InitGameOverWindow(GameObject popUpLayer, Health player, EnemySpawner enemySpawner)
        {
            PlayerDeath playerDeath = player.GetComponent<PlayerDeath>();
            GameOverWindow gameOverWindow = popUpLayer.GetComponentInChildren<GameOverWindow>(includeInactive: true);
            gameOverWindow.Initialize(playerDeath, enemySpawner);
        }
    }
}
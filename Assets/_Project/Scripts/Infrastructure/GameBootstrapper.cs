using System;
using _Project.Scripts.Infrastructure.Services.Factory.UIFactory;
using _Project.Scripts.Infrastructure.Services.GamePause;
using _Project.Scripts.Logic;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Logic.Weapon;
using _Project.Scripts.UI.Elements;
using _Project.Scripts.UI.Windows.PlayerStats;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.Infrastructure
{
    public class GameBootstrapper: IInitializable, IDisposable
    {
        private readonly IGamePauseService _pauseService;
        private readonly IUIFactory _uiFactory;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly PlayerSpawner _playerSpawner;
        private readonly EnemySpawner _enemySpawner;
        private readonly GameObject _hudLayerPrefab;
        private readonly GameObject _popUpLayerPrefab;
        private readonly Transform _enemySpawnPoint;
        private PlayerStatsPresenter _playerStatsPresenter;
        private Health _playerHealth;

        public GameBootstrapper(IGamePauseService pauseService, IUIFactory uiFactory, PlayerStatsModel playerStatsModel, 
            PlayerSpawner playerSpawner, EnemySpawner enemySpawner, GameObject hudLayerPrefab, 
            GameObject popUpLayerPrefab, Transform enemySpawnPoint)
        {
            _pauseService = pauseService;
            _uiFactory = uiFactory;
            _playerStatsModel = playerStatsModel;
            _playerSpawner = playerSpawner;
            _enemySpawner = enemySpawner;
            _hudLayerPrefab = hudLayerPrefab;
            _popUpLayerPrefab = popUpLayerPrefab;
            _enemySpawnPoint = enemySpawnPoint;
        }

        public void Initialize()
        {
            CursorController.SetCursorVisible(visible: false);
            
            GameObject hudLayer = _uiFactory.CreateHudLayer(_hudLayerPrefab);
            GameObject popUpLayer = _uiFactory.CreatePopUpLayer(_popUpLayerPrefab);
            
            _playerStatsModel.Initialize();
            PlayerStatsView playerStatsView = InitPlayerStatsView(popUpLayer, hudLayer);
            _playerStatsPresenter = InitPlayerStatsPresenter(playerStatsView, _playerStatsModel, _pauseService);
            
            _playerHealth = InitPlayer(_playerSpawner);
            InitPlayerHealthBarView(hudLayer);
            InitWeapon(_playerHealth.gameObject);
            
            InitEnemySpawner(_enemySpawner, _enemySpawnPoint, _playerHealth.transform);
        }

        public void Dispose() => 
            _playerStatsPresenter.Dispose();

        private Health InitPlayer(PlayerSpawner playerSpawner)
        {
            Health playerHealth = playerSpawner.Spawn();
            return playerHealth;
        }
        
        private void InitPlayerHealthBarView(GameObject hud)
        {
            HealthBarView playerHealthBarView = hud.GetComponentInChildren<HealthBarView>();
            playerHealthBarView.Construct(_playerHealth);
            playerHealthBarView.Initialize();
        }

        private void InitWeapon(GameObject player)
        {
            Weapon weapon = player.GetComponentInChildren<Weapon>();
            Camera playerCamera = player.GetComponentInChildren<Camera>();
            weapon.Initialize(playerCamera);
        }

        private void InitEnemySpawner(EnemySpawner enemySpawner, Transform target, Transform playerTransform) => 
            enemySpawner.SpawnAround(target, playerTransform);

        private PlayerStatsView InitPlayerStatsView(GameObject popUpLayer, GameObject hud)
        {
            Button openButton = hud.GetComponentInChildren<Button>();
            
            PlayerStatsView playerStatsView = popUpLayer.GetComponent<PlayerStatsView>();
            playerStatsView.Initialize(openButton);
            return playerStatsView;
        }

        private PlayerStatsPresenter InitPlayerStatsPresenter(PlayerStatsView view, PlayerStatsModel model , IGamePauseService pauseService)
        {
            PlayerStatsPresenter playerStatsPresenter = new PlayerStatsPresenter(view, model, pauseService);
            playerStatsPresenter.Initialize();
            return playerStatsPresenter;
        }
    }
}
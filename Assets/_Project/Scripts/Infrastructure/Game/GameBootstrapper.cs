using System;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player;
using _Project.Scripts.Logic.PlayerStats;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameBootstrapper : IInitializable
    {
        private readonly EffectsService _effectsService;
        private readonly PlayerSpawner _playerSpawner;
        private readonly EnemySpawner _enemySpawner;
        private readonly Transform _enemySpawnPoint;
        private readonly GameUIInitializer _uiInitializer;
        private readonly GameStarter _gameStarter;
        private readonly PlayerStatsModel _playerStatsModel;
        
        public GameBootstrapper(EffectsService effectsService, PlayerSpawner playerSpawner, EnemySpawner enemySpawner, 
            Transform enemySpawnPoint, GameUIInitializer uiInitializer, GameStarter gameStarter, PlayerStatsModel playerStatsModel)
        {
            _effectsService = effectsService;
            _playerSpawner = playerSpawner;
            _enemySpawner = enemySpawner;
            _enemySpawnPoint = enemySpawnPoint;
            _uiInitializer = uiInitializer;
            _gameStarter = gameStarter;
            _playerStatsModel = playerStatsModel;
        }

        public void Initialize() => 
            InitializeAsync().Forget();

        private async UniTaskVoid InitializeAsync()
        {
            try
            {
                await _effectsService.WarmUpAsync();
                _playerStatsModel.CreateStats();
            
                PlayerContainer player = await _playerSpawner.SpawnAsync();
            
                await _uiInitializer.InitUIAsync();
            
                _enemySpawner.SpawnAround( _enemySpawnPoint, player.transform);
            
                _gameStarter.StartGame();
            }
            catch (Exception e)
            {
                Debug.LogError($"[GAME BOOTSTRAPPER] init error: {e}");
            }
        }
    }
}
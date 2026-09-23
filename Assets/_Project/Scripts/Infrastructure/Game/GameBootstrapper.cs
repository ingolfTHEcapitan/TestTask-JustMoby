using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.PlayerStats.Data;
using _Project.Scripts.Logic.Spawners;
using _Project.Scripts.Services.Effects;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Game
{
    public class GameBootstrapper : IInitializable
    {
        private readonly IEffectsService _effectsService;
        private readonly PlayerSpawner _playerSpawner;
        private readonly EnemySpawner _enemySpawner;
        private readonly Transform _enemySpawnPoint;
        private readonly GameUIInitializer _uiInitializer;
        private readonly GameStarter _gameStarter;
        private readonly PlayerStatsData _playerStatsData;
        
        public GameBootstrapper(IEffectsService effectsService, PlayerSpawner playerSpawner, EnemySpawner enemySpawner, 
            Transform enemySpawnPoint, GameUIInitializer uiInitializer, GameStarter gameStarter, PlayerStatsData playerStatsData)
        {
            _effectsService = effectsService;
            _playerSpawner = playerSpawner;
            _enemySpawner = enemySpawner;
            _enemySpawnPoint = enemySpawnPoint;
            _uiInitializer = uiInitializer;
            _gameStarter = gameStarter;
            _playerStatsData = playerStatsData;
        }

        public async void Initialize()
        {
            await _effectsService.WarmUpAsync();
            await _playerStatsData.CreateStatsAsync();
            
            Health playerHealth = await _playerSpawner.SpawnAsync();
            
            await _uiInitializer.InitUIAsync(playerHealth);
            
            _enemySpawner.SpawnAround( _enemySpawnPoint, playerHealth.transform);
            
            _gameStarter.StartGame();
        }
    }
}
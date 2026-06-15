using System;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player.Stats;
using _Project.Scripts.Services.HealthCalculator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.Factory
{
    public class PlayerFactory : IPlayerFactory, IDisposable
    {
        private readonly DiContainer _container;
        private readonly IHealthCalculatorService _healthCalculator;
        private readonly IAssetProvider _assetProvider;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly Transform _gameParent;
        private Health _playerHealth;

        public PlayerFactory(DiContainer container, IHealthCalculatorService healthCalculator, 
            PlayerStatsModel playerStatsModel, Transform gameParent, IAssetProvider assetProvider)
        {
            _container = container;
            _healthCalculator = healthCalculator;
            _playerStatsModel = playerStatsModel;
            _gameParent = gameParent;
            _assetProvider = assetProvider;
        }

        public async UniTask<Health> CreatePlayer(Vector3 at)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.Player);
            _playerHealth = _container.InstantiatePrefabForComponent<Health>(prefab, at, Quaternion.identity, _gameParent);
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            _playerHealth.Initialize(maxHealth);
            
            PlayerStatData healthStat = _playerStatsModel.GetStat(StatName.Health); 
            healthStat.OnStatChanged += UpdatePlayerMaxHealth;
            
            _playerHealth.GetComponent<PlayerDeath>().Initialize();
            
            return _playerHealth;
        }

        public void Dispose() => 
            _playerStatsModel.OnStatsChanged -= UpdatePlayerMaxHealth;

        private void UpdatePlayerMaxHealth()
        {
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            _playerHealth.SetMaxHealth(maxHealth);
        }
    }
}
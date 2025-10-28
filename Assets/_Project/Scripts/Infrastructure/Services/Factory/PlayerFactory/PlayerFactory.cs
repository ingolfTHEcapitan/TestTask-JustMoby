using System;
using _Project.Scripts.Infrastructure.Services.HealthCalculator;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.PlayerStats;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Services.Factory.PlayerFactory
{
    public class PlayerFactory : IPlayerFactory, IDisposable
    {
        private readonly DiContainer _container;
        private readonly IHealthCalculatorService _healthCalculator;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly Transform _gameParent;
        private Health _playerHealth;

        public PlayerFactory(DiContainer container, IHealthCalculatorService healthCalculator, 
            PlayerStatsModel playerStatsModel, Transform gameParent)
        {
            _container = container;
            _healthCalculator = healthCalculator;
            _playerStatsModel = playerStatsModel;
            _gameParent = gameParent;
        }

        public Health CreatePlayer(GameObject prefab, Vector3 at)
        {
            _playerHealth = _container.InstantiatePrefabForComponent<Health>(prefab, at, Quaternion.identity, _gameParent);
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            _playerHealth.Initialize(maxHealth);
            
            _playerStatsModel.OnStatsChanged += UpdatePlayerMaxHealth;
            return _playerHealth;
        }

        public void Dispose() => 
            _playerStatsModel.OnStatsChanged -= UpdatePlayerMaxHealth;

        private void UpdatePlayerMaxHealth()
        {
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            _playerHealth.Initialize(maxHealth);
            _playerHealth.InvokeOnHealthChanged();
        }
    }
}
using System;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.Player.PlayerStats;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using _Project.Scripts.Services.HealthCalculator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player.Factory
{
    public class PlayerFactory : IPlayerFactory, IDisposable
    {
        private readonly IInstantiator _container;
        private readonly IHealthCalculatorService _healthCalculator;
        private readonly IAssetProvider _assetProvider;
        private readonly Transform _gameParent;
        private readonly PlayerStatsData _playerStatsData;
        private Health _playerHealth;
        private PlayerStatData _healthStat;

        public PlayerFactory(IInstantiator container, IHealthCalculatorService healthCalculator, PlayerStatsData playerStatsData, Transform gameParent, IAssetProvider assetProvider)
        {
            _container = container;
            _healthCalculator = healthCalculator;
            _playerStatsData = playerStatsData;
            _gameParent = gameParent;
            _assetProvider = assetProvider;
        }

        public async UniTask<Health> CreatePlayerAsync(Vector3 at)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.Player);
            _playerHealth = _container.InstantiatePrefabForComponent<Health>(prefab, at, Quaternion.identity, _gameParent);
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            _playerHealth.Initialize(maxHealth);
            
            _healthStat = _playerStatsData.GetStat(StatName.Health); 
            _healthStat.OnStatChanged += UpdatePlayerMaxHealth;
            
            _playerHealth.GetComponent<PlayerDeath>().Initialize();

            InitWeapon(_playerHealth);
            
            return _playerHealth;
        }

        public void Dispose() => 
            _healthStat.OnStatChanged -= UpdatePlayerMaxHealth;

        private void UpdatePlayerMaxHealth()
        {
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            _playerHealth.SetMaxHealth(maxHealth);
        }
        
        private void InitWeapon(Health player)
        {
            Weapon.Weapon weapon = player.GetComponentInChildren<Weapon.Weapon>();
            Camera playerCamera = player.GetComponentInChildren<Camera>();
            weapon.Initialize(playerCamera);
        }
    }
}
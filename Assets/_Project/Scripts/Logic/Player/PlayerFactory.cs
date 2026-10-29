using System;
using System.Data;
using _Project.Scripts.Infrastructure.AssetManagement;
using _Project.Scripts.Infrastructure.Game;
using _Project.Scripts.Logic.Common;
using _Project.Scripts.Logic.PlayerStats;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player
{
    public class PlayerFactory : IDisposable
    {
        private readonly IInstantiator _container;
        private readonly HealthCalculator _healthCalculator;
        private readonly IAssetProvider _assetProvider;
        private readonly Transform _gameParent;
        private readonly PlayerStatsModel _playerStatsModel;
        private PlayerContainer _player;
        private PlayerStatData _healthStat;

        public PlayerFactory(IInstantiator container, HealthCalculator healthCalculator, PlayerStatsModel playerStatsModel, Transform gameParent, IAssetProvider assetProvider)
        {
            _container = container;
            _healthCalculator = healthCalculator;
            _playerStatsModel = playerStatsModel;
            _gameParent = gameParent;
            _assetProvider = assetProvider;
        }

        public async UniTask<PlayerContainer> CreatePlayerAsync(Vector3 at)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(AssetAddress.Player);
            _player = _container.InstantiatePrefabForComponent<PlayerContainer>(prefab, at, Quaternion.identity, _gameParent);
            
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            _player.Health.Initialize(maxHealth);
            
            _playerStatsModel.TryGetStat(StatName.Health, out _healthStat); 
            _healthStat.OnStatChanged += UpdatePlayerMaxHealth;
            
            _player.PlayerDeath.Initialize();
            _player.Weapon.Initialize(_player.Camera);;
            
            return _player;
        }

        public PlayerContainer GetPlayer()
        {
            if (_player)
                return _player;
            
            throw new InvalidConstraintException
                ($"{_player.gameObject.name} requested before creation. Ensure it created before resolution");
        }
        
        public void Dispose() => 
            _healthStat.OnStatChanged -= UpdatePlayerMaxHealth;

        private void UpdatePlayerMaxHealth()
        {
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            _player.Health.SetMaxHealth(maxHealth);
        }
    }
}
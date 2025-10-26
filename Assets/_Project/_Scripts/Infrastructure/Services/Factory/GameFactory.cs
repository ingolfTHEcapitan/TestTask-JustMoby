using _Project._Scripts.Configs;
using _Project._Scripts.Configs.Weapon;
using _Project._Scripts.Enemy;
using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.PlayerInput;
using _Project._Scripts.Logic;
using _Project._Scripts.Logic.PlayerStats;
using _Project._Scripts.Logic.Weapon;
using _Project._Scripts.Player;
using _Project._Scripts.UI.Elements;
using UnityEngine;

namespace _Project._Scripts.Infrastructure.Services.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IGamePauseService _pauseService;
        private readonly IInputService _inputService;
        private readonly HealthCalculator _healthCalculator;
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly Transform _dynamicObjectsParent;
        private readonly GameObject _hudPrefab;
        private readonly GameObject _popUpLayerPrefab;
        
        public GameFactory(IGamePauseService pauseService, IInputService inputService, 
            HealthCalculator healthCalculator, PlayerStatsModel playerStatsModel, 
            Transform dynamicObjectsParent, GameObject hudPrefab, GameObject popUpLayerPrefab)
        {
            _hudPrefab = hudPrefab;
            _popUpLayerPrefab = popUpLayerPrefab;
            _pauseService = pauseService;
            _inputService = inputService;
            _healthCalculator = healthCalculator;
            _playerStatsModel = playerStatsModel;
            _dynamicObjectsParent = dynamicObjectsParent;
        }
        

        public GameObject CreateEnemy(EnemySpawnerConfig config, Vector3 spawnPoint)
        {
            GameObject enemy = Object.Instantiate(config.Prefab, spawnPoint, Quaternion.identity, _dynamicObjectsParent);

            enemy.GetComponent<EnemyDeath>().Construct(_playerStatsModel);
            
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            enemyMovement.Construct(_pauseService);
            enemyMovement.Initialize(spawnPoint);
                
            float maxHealth = _healthCalculator.CalculateEnemyMaxHealth();
            Health health = enemy.GetComponent<Health>();
            health.Initialize(maxHealth);
            
            HealthBarView healthBar = enemy.GetComponentInChildren<HealthBarView>();
            healthBar.Construct(health);
            healthBar.Initialize();
            return enemy;
        }

        public GameObject CreatePlayer(GameObject prefab, Vector3 at, Transform parent)
        {
            GameObject player = Object.Instantiate(prefab, at, Quaternion.identity, parent.transform);
            
            float maxHealth = _healthCalculator.CalculatePlayerMaxHealth();
            Health health = player.GetComponent<Health>();
            health.Initialize(maxHealth);
            
            player.GetComponent<PlayerCameraLook>().Construct(_pauseService, _inputService);
            player.GetComponent<PlayerMovement>().Construct(_playerStatsModel, _pauseService, _inputService);
            return player;
        }
        
        public GameObject CreateHud(Transform parent) => 
            Object.Instantiate(_hudPrefab, parent);
        
        public GameObject CreatePopUpLayer(Transform parent) => 
            Object.Instantiate(_popUpLayerPrefab, parent);

        public Bullet CreateBullet(BulletConfig config, Transform at, Vector3 shootDirection)
        {
            float damage = _playerStatsModel.GetStatValue(StatName.Damage);
            
            Bullet bullet = Object.Instantiate(config.Prefab, at).GetComponent<Bullet>();
            bullet.Initialize(config, shootDirection, damage, _dynamicObjectsParent);
            return bullet;
        }
    }
}
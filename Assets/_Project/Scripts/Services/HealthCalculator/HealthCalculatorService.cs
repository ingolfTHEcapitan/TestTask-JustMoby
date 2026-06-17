using _Project.Scripts.Logic.Player.PlayerStats;
using _Project.Scripts.Logic.Player.PlayerStats.Data;

namespace _Project.Scripts.Services.HealthCalculator
{
    public class HealthCalculatorService : IHealthCalculatorService
    {
        private readonly PlayerStatsData _playerStatsData;

        public HealthCalculatorService(PlayerStatsData playerStatsData) => 
            _playerStatsData = playerStatsData;

        public float CalculateEnemyMaxHealth()
        {
            PlayerStatData damageStat = _playerStatsData.Stats[StatName.Damage];

            int minShotsToKill = 1;
            int maxShotsToKill = 10;

            int randomShootsCount = UnityEngine.Random.Range(minShotsToKill, maxShotsToKill + 1);
            float maxHealth = damageStat.BaseValue * randomShootsCount;
            return maxHealth;
        }

        public float CalculatePlayerMaxHealth() => 
            _playerStatsData.GetStatValue(StatName.Health);
    }
}
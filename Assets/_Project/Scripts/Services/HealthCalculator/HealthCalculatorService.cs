using _Project.Scripts.Logic.Player.PlayerStats;
using _Project.Scripts.Logic.Player.PlayerStats.Data;

namespace _Project.Scripts.Services.HealthCalculator
{
    public class HealthCalculatorService : IHealthCalculatorService
    {
        private const int MinShotsToKill = 1;
        private const int MaxShotsToKill = 10;
        private const int _exclusiveOffset = 1;

        private readonly PlayerStatsData _playerStatsData;

        public HealthCalculatorService(PlayerStatsData playerStatsData) => 
            _playerStatsData = playerStatsData;

        public float CalculateEnemyMaxHealth()
        {
            PlayerStatData damageStat = _playerStatsData.GetStat(StatName.Damage);
            
            int randomShootsCount = UnityEngine.Random.Range(MinShotsToKill, MaxShotsToKill + _exclusiveOffset);
            float maxHealth = damageStat.BaseValue * randomShootsCount;
            return maxHealth;
        }

        public float CalculatePlayerMaxHealth() => 
            _playerStatsData.GetStatValue(StatName.Health);
    }
}
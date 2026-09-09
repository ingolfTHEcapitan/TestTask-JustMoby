using _Project.Scripts.Logic.PlayerStats;

namespace _Project.Scripts.Services.HealthCalculator
{
    public class HealthCalculatorService : IHealthCalculatorService
    {
        private const int MinShotsToKill = 1;
        private const int MaxShotsToKill = 10;
        private const int ExclusiveOffset = 1;

        private readonly PlayerStatsModel _playerStatsModel;

        public HealthCalculatorService(PlayerStatsModel playerStatsModel) => 
            _playerStatsModel = playerStatsModel;

        public float CalculateEnemyMaxHealth()
        {
            PlayerStatData damageStat = _playerStatsModel.GetStat(StatName.Damage);
            
            int randomShootsCount = UnityEngine.Random.Range(MinShotsToKill, MaxShotsToKill + ExclusiveOffset);
            float maxHealth = damageStat.BaseValue * randomShootsCount;
            return maxHealth;
        }

        public float CalculatePlayerMaxHealth() => 
            _playerStatsModel.GetStatValue(StatName.Health);
    }
}
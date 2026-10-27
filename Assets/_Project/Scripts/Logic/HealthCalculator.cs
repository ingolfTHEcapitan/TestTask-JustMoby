using _Project.Scripts.Logic.PlayerStats;

namespace _Project.Scripts.Logic
{
    public class HealthCalculator
    {
        private const int MinShotsToKill = 1;
        private const int MaxShotsToKill = 10;
        private const int ExclusiveOffset = 1;

        private readonly PlayerStatsModel _playerStatsModel;

        public HealthCalculator(PlayerStatsModel playerStatsModel) => 
            _playerStatsModel = playerStatsModel;

        public float CalculateEnemyMaxHealth()
        { 
            _playerStatsModel.TryGetStat(StatName.Damage, out PlayerStatData damageStat);
            
            int randomShootsCount = UnityEngine.Random.Range(MinShotsToKill, MaxShotsToKill + ExclusiveOffset);
            float maxHealth = damageStat.BaseValue * randomShootsCount;
            return maxHealth;
        }

        public float CalculatePlayerMaxHealth() => 
            _playerStatsModel.GetStatValue(StatName.Health);
    }
}
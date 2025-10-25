using _Project._Scripts.Logic.PlayerStats;

namespace _Project._Scripts.Logic
{
    public class HealthCalculator
    {
        private readonly PlayerStatsModel _playerStatsModel;

        public HealthCalculator(PlayerStatsModel playerStatsModel) => 
            _playerStatsModel = playerStatsModel;

        public float CalculateEnemyMaxHealth()
        {
            PlayerStatData damageStat = _playerStatsModel.Stats[StatName.Damage];

            int minShotsToKill = 1;
            int maxShotsToKill = 10;

            int randomShootsCount = UnityEngine.Random.Range(minShotsToKill, maxShotsToKill + 1);
            float maxHealth = damageStat.BaseValue * randomShootsCount;
            return maxHealth;
        }

        public float CalculatePlayerMaxHealth() => 
            _playerStatsModel.GetStatValue(StatName.Health);
    }
}
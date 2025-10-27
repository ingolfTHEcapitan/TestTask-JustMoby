namespace _Project._Scripts.Infrastructure.Services.HealthCalculator
{
    public interface IHealthCalculatorService
    {
        float CalculateEnemyMaxHealth();
        float CalculatePlayerMaxHealth();
    }
}
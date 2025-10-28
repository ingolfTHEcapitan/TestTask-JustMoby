namespace _Project.Scripts.Infrastructure.Services.HealthCalculator
{
    public interface IHealthCalculatorService
    {
        float CalculateEnemyMaxHealth();
        float CalculatePlayerMaxHealth();
    }
}
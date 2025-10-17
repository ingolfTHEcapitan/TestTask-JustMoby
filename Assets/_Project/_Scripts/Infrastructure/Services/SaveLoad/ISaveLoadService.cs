using _Project._Scripts.Data;

namespace _Project._Scripts.Infrastructure.Services.SaveLoad
{
    public interface ISaveLoadService
    {
        void SaveProgress(PlayerProgress playerProgress);
        PlayerProgress LoadProgress();
    }
}
using _Project.Scripts.Data;

namespace _Project.Scripts.Infrastructure.Services.SaveLoad
{
    public interface ISaveLoadService
    {
        void SaveProgress(PlayerProgress playerProgress);
        PlayerProgress LoadProgress();
    }
}
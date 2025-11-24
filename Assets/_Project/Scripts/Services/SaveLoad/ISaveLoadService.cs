using _Project.Scripts.Data;

namespace _Project.Scripts.Services.SaveLoad
{
    public interface ISaveLoadService
    {
        void SaveProgress(PlayerProgress playerProgress);
        PlayerProgress LoadProgress();
    }
}
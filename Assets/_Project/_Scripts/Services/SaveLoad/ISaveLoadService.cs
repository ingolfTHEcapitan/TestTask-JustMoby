using _Project._Scripts.Data;

namespace _Project._Scripts.SaveLoad
{
    public interface ISaveLoadService
    {
        void SaveProgress(PlayerProgress playerProgress);
        PlayerProgress LoadProgress();
    }
}
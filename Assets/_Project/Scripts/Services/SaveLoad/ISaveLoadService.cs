using _Project.Scripts.Data;
using _Project.Scripts.Services.Progress;

namespace _Project.Scripts.Services.SaveLoad
{
    public interface ISaveLoadService
    {
        void SaveProgress(IProgressService progressService);
        PlayerProgress LoadProgress();
    }
}
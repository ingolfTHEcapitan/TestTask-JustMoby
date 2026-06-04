using _Project.Scripts.Data;
using _Project.Scripts.Services.Progress;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.SaveLoad
{
    public interface ISaveLoadService
    {
        UniTask SaveProgressAsync(IProgressService progressService);
        UniTask<PlayerProgress> LoadProgressAsync();
    }
}
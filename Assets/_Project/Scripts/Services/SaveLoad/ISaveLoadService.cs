using _Project.Scripts.Data.Player;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.SaveLoad
{
    public interface ISaveLoadService
    {
        UniTask SaveProgressAsync(PlayerProgress playerProgress);
        UniTask<PlayerProgress> LoadProgressAsync();
    }
}
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.SceneLoader
{
    public interface ISceneLoaderService
    {
        UniTask LoadAsync(string sceneName);
        UniTask LoadAsync(int buildIndex);
    }
}
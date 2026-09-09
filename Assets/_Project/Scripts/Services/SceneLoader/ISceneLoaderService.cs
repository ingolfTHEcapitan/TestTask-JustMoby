using System;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.SceneLoader
{
    public interface ISceneLoaderService
    {
        UniTask LoadAsync(int buildIndex);
        UniTask ReloadAsync();
        event Action BeforeSceneUnload;
        event Action AfterSceneLoaded;
    }
}
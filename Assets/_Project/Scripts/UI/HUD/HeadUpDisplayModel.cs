using System.Threading.Tasks;
using _Project.Scripts.Services.LoadingCurtain;
using _Project.Scripts.Services.SceneLoader;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayModel
    {
        private readonly ILoadingCurtainService _loadingCurtain;
        private readonly ISceneLoaderService _sceneLoader;

        public HeadUpDisplayModel(ILoadingCurtainService loadingCurtain, ISceneLoaderService sceneLoader)
        {
            _loadingCurtain = loadingCurtain;
            _sceneLoader = sceneLoader;
        }

        public async UniTask LoadMainMenu()
        {
            await _loadingCurtain.ShowLoadingAsync();
            await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.MainMenu);
        }
    }
}
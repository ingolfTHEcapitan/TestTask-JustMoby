using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayModel
    {
        private readonly LoadingCurtainPresenter _loadingCurtainPresenter;
        private readonly ISceneLoaderService _sceneLoader;

        public HeadUpDisplayModel(LoadingCurtainPresenter loadingCurtainPresenter, ISceneLoaderService sceneLoader)
        {
            _loadingCurtainPresenter = loadingCurtainPresenter;
            _sceneLoader = sceneLoader;
        }

        public async UniTask LoadMainMenu()
        {
            _loadingCurtainPresenter.ShowLoading();
            await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.MainMenu);
        }
    }
}
using _Project.Scripts.Services;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.Services.SceneLoader;
using _Project.Scripts.UI.Windows.LoadingCurtain;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.UI.HUD
{
    public class HeadUpDisplayModel
    {
        private readonly LoadingCurtainPresenter _loadingCurtainPresenter;
        private readonly SceneLoaderService _sceneLoader;
        private readonly IGamePauseService _pauseService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly ProgressService _progressService;
        
        public bool GameIsPaused => _pauseService.IsPaused;

        public HeadUpDisplayModel(LoadingCurtainPresenter loadingCurtainPresenter, SceneLoaderService sceneLoader,
            IGamePauseService pauseService, [Inject(Id = SaveType.Coordinator)]ISaveLoadService saveLoadService, 
            ProgressService progressService)
        {
            _loadingCurtainPresenter = loadingCurtainPresenter;
            _sceneLoader = sceneLoader;
            _pauseService = pauseService;
            _saveLoadService = saveLoadService;
            _progressService = progressService;
        }

        public async UniTask LoadMainMenuAsync()
        {
            _loadingCurtainPresenter.ShowLoading();
            await _saveLoadService.SaveProgressAsync(_progressService);
            await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.MainMenu);
        }
    }
}
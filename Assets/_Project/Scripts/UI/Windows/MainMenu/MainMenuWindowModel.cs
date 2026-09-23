using _Project.Scripts.Services.SceneLoader;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.UI.Windows.MainMenu
{
    public class MainMenuWindowModel
    {
        private readonly ISceneLoaderService _sceneLoader;

        public MainMenuWindowModel(ISceneLoaderService sceneLoader) => 
            _sceneLoader = sceneLoader;

        public async UniTask LoadGameplayScene() => 
            await _sceneLoader.LoadAsync(buildIndex: (int)SceneName.Gameplay);
        
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
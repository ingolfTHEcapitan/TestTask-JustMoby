using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Services.SceneLoader
{
    public class SceneLoaderService : ISceneLoaderService
    {
        public async UniTask LoadAsync(string sceneName)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
                return;
            
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask();
        }
        
        public async UniTask LoadAsync(int buildIndex)
        {
            if (SceneManager.GetActiveScene().buildIndex == buildIndex)
                return;
            
            await SceneManager.LoadSceneAsync(buildIndex).ToUniTask();
        }
        
        public async UniTask ReloadAsync()
        {
            int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            await SceneManager.LoadSceneAsync(sceneBuildIndex).ToUniTask();
        }
    }
}
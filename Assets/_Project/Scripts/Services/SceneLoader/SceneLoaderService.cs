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
    }
}
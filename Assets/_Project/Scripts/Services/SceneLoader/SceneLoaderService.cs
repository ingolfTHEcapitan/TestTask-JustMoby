using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Services.SceneLoader
{
    public class SceneLoaderService : ISceneLoaderService
    {
        public event Action BeforeSceneUnload;
        public event Action AfterSceneLoaded;
        
        public async UniTask LoadAsync(int buildIndex)
        {
            if (SceneManager.GetActiveScene().buildIndex == buildIndex)
                return;

            await LoadSceneAsync(buildIndex);
        }

        public async UniTask ReloadAsync()
        {
            int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            await LoadSceneAsync(sceneBuildIndex);
        }

        private async Task LoadSceneAsync(int buildIndex)
        {
            BeforeSceneUnload?.Invoke();
            await SceneManager.LoadSceneAsync(buildIndex).ToUniTask();
            AfterSceneLoaded?.Invoke();
        }
    }
}
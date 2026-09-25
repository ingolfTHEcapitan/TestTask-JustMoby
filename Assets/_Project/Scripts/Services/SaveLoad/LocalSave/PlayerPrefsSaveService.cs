using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.Progress;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services.SaveLoad.LocalSave
{
    public class PlayerPrefsSaveService: ISaveLoadService
    {
        private const string PlayerProgressKey = "PlayerProgress";
        
        public UniTask SaveProgressAsync(IProgressService progressService)
        {
            string json = JsonUtility.ToJson(progressService.PlayerProgress, false);
            PlayerPrefs.SetString(PlayerProgressKey, json);
            return UniTask.CompletedTask;
        }

        public async UniTask<PlayerProgress> LoadProgressAsync()
        {
            if (PlayerPrefs.HasKey(PlayerProgressKey))
            {
                string json = PlayerPrefs.GetString(PlayerProgressKey);
                PlayerProgress playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                return await UniTask.FromResult(playerProgress);
            }
            
            return await UniTask.FromResult(new PlayerProgress());
        }
    }
}
using System;
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
            progressService.PlayerProgress.LastSaveTimeUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            string json = JsonUtility.ToJson(progressService.PlayerProgress, false);
            PlayerPrefs.SetString(PlayerProgressKey, json);
            Debug.Log("Progress saved to PlayerPrefs");
            return UniTask.CompletedTask;
        }

        public UniTask<PlayerProgress> LoadProgressAsync()
        {
            if (PlayerPrefs.HasKey(PlayerProgressKey))
            {
                string json = PlayerPrefs.GetString(PlayerProgressKey);
                PlayerProgress playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                Debug.Log("Progress loaded from PlayerPrefs");
                return  UniTask.FromResult(playerProgress);
            }
            
            return UniTask.FromResult(new PlayerProgress());
        }
    }
}
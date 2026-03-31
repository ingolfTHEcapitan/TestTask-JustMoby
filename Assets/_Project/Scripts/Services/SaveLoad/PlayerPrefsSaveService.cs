using _Project.Scripts.Data;
using _Project.Scripts.Services.Progress;
using UnityEngine;

namespace _Project.Scripts.Services.SaveLoad
{
    public class PlayerPrefsSaveService: ISaveLoadService
    {
        private const string PlayerProgressKey = "PlayerProgress";
        
        public void SaveProgress(IProgressService progressService)
        {
            string json = JsonUtility.ToJson(progressService.PlayerProgress, false);
            PlayerPrefs.SetString(PlayerProgressKey, json);
            Debug.Log("Progress saved to PlayerPrefs");
        }

        public PlayerProgress LoadProgress()
        {
            if (PlayerPrefs.HasKey(PlayerProgressKey))
            {
                string json = PlayerPrefs.GetString(PlayerProgressKey);
                PlayerProgress playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                Debug.Log("Progress loaded from PlayerPrefs");
                return  playerProgress;
            }
            
            return new PlayerProgress();
        }
    }
}
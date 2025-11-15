using _Project.Scripts.Data;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.SaveLoad
{
    public class PlayerPrefsSaveLoadService: ISaveLoadService
    {
        private const string PlayerProgressKey = "PlayerProgress";
        
        public void SaveProgress(PlayerProgress playerProgress)
        {
            string json = JsonUtility.ToJson(playerProgress, false);
            PlayerPrefs.SetString(PlayerProgressKey, json);
            Debug.Log("Progress saved to PlayerPrefs");
        }

        public PlayerProgress LoadProgress()
        {
            PlayerProgress playerProgress = new PlayerProgress();
            
            if (PlayerPrefs.HasKey(PlayerProgressKey))
            {
                string json = PlayerPrefs.GetString(PlayerProgressKey);
                playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                Debug.Log("Progress loaded from PlayerPrefs");
                return  playerProgress;
            }
            
            SaveProgress(playerProgress);
            return null;
        }
    }
}
using System.IO;
using _Project._Scripts.Data;
using UnityEngine;

namespace _Project._Scripts.SaveLoad
{
    public class SaveLoad
    {
        private readonly string SaveDirectoryPath;
        private readonly string SavePath; 
        
        public SaveLoad()
        {
            SaveDirectoryPath = Path.Combine(Application.dataPath, "Save");
            SavePath = Path.Combine(SaveDirectoryPath, "Save.json");
        }
        
        public void SaveProgress(PlayerProgress playerProgress)
        {
            if (!Directory.Exists(SaveDirectoryPath)) 
                Directory.CreateDirectory(SaveDirectoryPath);
            
            string json = JsonUtility.ToJson(playerProgress, prettyPrint: true);
            File.WriteAllText(SavePath, json);
        }

        public PlayerProgress LoadProgress()
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                PlayerProgress playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                return playerProgress;
            }
            
            PlayerProgress newPlayerProgress = new PlayerProgress();
            SaveProgress(newPlayerProgress);
            return newPlayerProgress;
        }
    }
}
using System.IO;
using _Project._Scripts.Data;
using UnityEngine;

namespace _Project._Scripts.SaveLoad
{
    public class SaveLoadService
    {
        private const string FolderName = "Saves";
        private const string FileName = "Save.json";

        private readonly string SaveDirectoryPath;
        private readonly string SavePath;

        public SaveLoadService()
        {
            SaveDirectoryPath = Path.Combine(Application.dataPath, FolderName);
            SavePath = Path.Combine(SaveDirectoryPath, FileName);
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
            PlayerProgress playerProgress = new PlayerProgress();
            
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                return playerProgress;
            }
            
            SaveProgress(playerProgress);
            return null;
        }
    }
}
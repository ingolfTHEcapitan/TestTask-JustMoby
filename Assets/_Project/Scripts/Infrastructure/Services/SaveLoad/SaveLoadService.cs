using System.IO;
using _Project.Scripts.Data;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.SaveLoad
{
    public class SaveLoadService : ISaveLoadService
    {
        private const string FolderName = "Saves";
        private const string FileName = "Save.json";

        private readonly string _saveDirectoryPath;
        private readonly string _savePath;

        public SaveLoadService()
        {
            _saveDirectoryPath = Path.Combine(Application.dataPath, FolderName);
            _savePath = Path.Combine(_saveDirectoryPath, FileName);
        }
        
        public void SaveProgress(PlayerProgress playerProgress)
        {
            if (!Directory.Exists(_saveDirectoryPath)) 
                Directory.CreateDirectory(_saveDirectoryPath);
            
            string json = JsonUtility.ToJson(playerProgress, prettyPrint: true);
            File.WriteAllText(_savePath, json);
            Debug.Log("Progress saved to " + _savePath);
        }

        public PlayerProgress LoadProgress()
        {
            PlayerProgress playerProgress = new PlayerProgress();
            
            if (File.Exists(_savePath))
            {
                string json = File.ReadAllText(_savePath);
                playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                Debug.Log("Progress loaded from " + _savePath);
                return playerProgress;
            }
            
            SaveProgress(playerProgress);
            return null;
        }
    }
}
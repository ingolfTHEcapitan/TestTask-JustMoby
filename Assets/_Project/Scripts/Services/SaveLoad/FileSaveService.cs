using System.IO;
using _Project.Scripts.Data;
using UnityEngine;

namespace _Project.Scripts.Services.SaveLoad
{
    public class FileSaveService : ISaveLoadService
    {
        private const string FolderName = "Saves";
        private const string FileName = "Save.json";

        private readonly string _saveDirectoryPath;
        private readonly string _savePath;

        public FileSaveService()
        {
            _saveDirectoryPath = Path.Combine(Application.persistentDataPath, FolderName);
            _savePath = Path.Combine(_saveDirectoryPath, FileName);
        }
        
        public void SaveProgress(PlayerProgress playerProgress)
        {
            if (!Directory.Exists(_saveDirectoryPath)) 
                Directory.CreateDirectory(_saveDirectoryPath);
            
            string json = JsonUtility.ToJson(playerProgress, prettyPrint: true);
            File.WriteAllText(_savePath, json);
            Debug.Log("Progress saved to File, save path: " + _savePath);
        }

        public PlayerProgress LoadProgress()
        {
            PlayerProgress playerProgress = new PlayerProgress();
            
            if (File.Exists(_savePath))
            {
                string json = File.ReadAllText(_savePath);
                playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                Debug.Log("Progress loaded from File, save path: " + _savePath);
                return playerProgress;
            }
            
            SaveProgress(playerProgress);
            return null;
        }
    }
}
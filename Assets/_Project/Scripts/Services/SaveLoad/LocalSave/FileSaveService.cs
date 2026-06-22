using System;
using System.IO;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.Progress;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services.SaveLoad.LocalSave
{
    public class FileSaveService : ISaveLoadService
    {
        public static readonly string FolderName = "Saves";
        private const string FileName = "Save.json";

        private readonly string _saveDirectoryPath;
        private readonly string _savePath;

        public FileSaveService()
        {
            _saveDirectoryPath = Path.Combine(Application.persistentDataPath, FolderName);
            _savePath = Path.Combine(_saveDirectoryPath, FileName);
        }
        
        public async UniTask SaveProgressAsync(IProgressService progressService)
        {
            if (!Directory.Exists(_saveDirectoryPath)) 
                Directory.CreateDirectory(_saveDirectoryPath);
            
            progressService.PlayerProgress.LastSaveTimeUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            string json = JsonUtility.ToJson(progressService.PlayerProgress, prettyPrint: true);
            await File.WriteAllTextAsync(_savePath, json).AsUniTask();
            Debug.Log("Progress saved to File, save path: " + _savePath);
        }

        public async UniTask<PlayerProgress> LoadProgressAsync()
        {
            if (File.Exists(_savePath))
            {
                string json = await File.ReadAllTextAsync(_savePath).AsUniTask();
                PlayerProgress playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                Debug.Log("Progress loaded from File, save path: " + _savePath);
                return playerProgress;
            }
            
            return new PlayerProgress();
        }
    }
}
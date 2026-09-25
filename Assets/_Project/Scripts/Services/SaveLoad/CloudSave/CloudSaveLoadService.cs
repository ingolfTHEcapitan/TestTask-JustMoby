using System.Collections.Generic;
using _Project.Scripts.Data.Player;
using _Project.Scripts.Services.Progress;
using Cysharp.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace _Project.Scripts.Services.SaveLoad.CloudSave
{
    public class CloudSaveLoadService : ISaveLoadService
    {
        private const string CloudSaveKey = "PlayerProgress";
        
        public async UniTask SaveProgressAsync(IProgressService progressService)
        {
            string json = JsonUtility.ToJson(progressService.PlayerProgress, false);
            
            Dictionary<string, object> playerProgress = new Dictionary<string, object>
            { { CloudSaveKey, json }, };
            
            await CloudSaveService.Instance.Data.Player.SaveAsync(playerProgress);
        }

        public async UniTask<PlayerProgress> LoadProgressAsync()
        {
            Dictionary<string, Item> playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(
                new HashSet<string> { CloudSaveKey });

            if (playerData.TryGetValue(CloudSaveKey, out Item playerProgress))
            {
                string json = playerProgress.Value.GetAs<string>();
                return JsonUtility.FromJson<PlayerProgress>(json);
            }
            
            return new PlayerProgress();
        }
    }
}
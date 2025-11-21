using System;
using _Project.Scripts.Infrastructure.Services.SaveLoad;
using UnityEngine;

namespace _Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "SaveLoadServiceConfig", menuName = "Configs/SaveLoadServiceConfig")]
    public class SaveLoadServiceConfig: ScriptableObject
    {
        [SerializeField] private SaveLoadServiceType saveLoadServiceType = SaveLoadServiceType.PlayerPrefs;
        
        public ISaveLoadService GetInstance()
        {
            return saveLoadServiceType switch
            {
                SaveLoadServiceType.Json => new JsonSaveLoadService(),
                SaveLoadServiceType.PlayerPrefs => new PlayerPrefsSaveLoadService(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
using System;
using _Project.Scripts.Services.SaveLoad;
using UnityEngine;

namespace _Project.Scripts.Configs
{
    [Serializable]
    public class SaveServiceConfig
    {
        [SerializeField] private SaveServiceType saveServiceType = SaveServiceType.PlayerPrefs;
        
        public ISaveLoadService GetInstance()
        {
            return saveServiceType switch
            {
                SaveServiceType.File => new FileSaveService(),
                SaveServiceType.PlayerPrefs => new PlayerPrefsSaveService(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
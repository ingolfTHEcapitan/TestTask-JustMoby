using System;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.Services.SaveLoad.LocalSave;
using UnityEngine;

namespace _Project.Scripts.Configs
{
    [Serializable]
    public class LocalSaveServiceConfig
    {
        [SerializeField] private LocalSaveType saveType = LocalSaveType.PlayerPrefs;
        
        public ISaveLoadService GetInstance()
        {
            return saveType switch
            {
                LocalSaveType.File => new FileSaveService(),
                LocalSaveType.PlayerPrefs => new PlayerPrefsSaveService(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
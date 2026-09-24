using System;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.Services.SaveLoad.LocalSave;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Configs
{
    [Serializable]
    public class LocalSaveServiceConfig
    {
        [SerializeField] private LocalSaveType saveType = LocalSaveType.PlayerPrefs;

        [Inject]
        private void Construct()
        {
            
        }
        
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
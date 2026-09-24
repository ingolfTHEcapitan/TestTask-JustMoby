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
        private IInstantiator _container;

        [Inject]
        private void Construct(IInstantiator container) => 
            _container = container;

        public ISaveLoadService GetInstance()
        {
            return saveType switch
            {
                LocalSaveType.File => _container.Instantiate<FileSaveService>(),
                LocalSaveType.PlayerPrefs => _container.Instantiate<PlayerPrefsSaveService>(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
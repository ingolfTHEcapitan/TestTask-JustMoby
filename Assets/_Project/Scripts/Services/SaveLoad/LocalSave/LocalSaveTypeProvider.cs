using System;
using _Project.Scripts.Configs;
using Zenject;

namespace _Project.Scripts.Services.SaveLoad.LocalSave
{
    public class LocalSaveTypeProvider
    {
        private readonly IInstantiator _container;
        private readonly LocalSaveTypeConfig _config;

        public LocalSaveTypeProvider(IInstantiator container, LocalSaveTypeConfig config)
        {
            _container = container;
            _config = config;
        }

        public ISaveLoadService GetInstance()
        {
            return _config.SaveType switch
            {
                LocalSaveType.File => _container.Instantiate<FileSaveService>(),
                LocalSaveType.PlayerPrefs => _container.Instantiate<PlayerPrefsSaveService>(),
                _ => throw new ArgumentOutOfRangeException(_config.SaveType.ToString(), 
                    $"[LocalSaveTypeProvider] Wasn't found local save realisation for type {_config.SaveType}.")
            };
        }
    }
}
using System;
using _Project.Scripts.Services.SaveLoad;
using _Project.Scripts.Services.SaveLoad.LocalSave;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Configs
{
    [Serializable]
    public class LocalSaveTypeConfig
    {
        [SerializeField] private LocalSaveType saveType = LocalSaveType.PlayerPrefs;

        public LocalSaveType SaveType => saveType;
    }
}
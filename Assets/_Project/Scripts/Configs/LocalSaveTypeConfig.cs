using System;
using _Project.Scripts.Services.SaveLoad.LocalSave;
using UnityEngine;

namespace _Project.Scripts.Configs
{
    [Serializable]
    public class LocalSaveTypeConfig
    {
        [field: SerializeField] public LocalSaveType SaveType { get; private set; } = LocalSaveType.None;
    }
}
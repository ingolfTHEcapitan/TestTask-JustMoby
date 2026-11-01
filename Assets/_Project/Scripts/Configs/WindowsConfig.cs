using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "WindowsConfig", menuName = "Configs/WindowsConfig", order = 0)]
    public class WindowsConfig : ScriptableObject
    {
        [Serializable]
        private class WindowMap
        {
            public WindowId WindowId;
            public AssetReferenceGameObject WindowAssetReference;
        }
        
        [SerializeField] private List<WindowMap> WindowList;
        
        private Dictionary<WindowId, AssetReferenceGameObject> _windowDictionary = new Dictionary<WindowId, AssetReferenceGameObject>();

        public void Initialize() => 
            _windowDictionary = WindowList.ToDictionary(x => x.WindowId, x => x.WindowAssetReference);

        public AssetReferenceGameObject GetAssetReference(WindowId id)
        {
            if (_windowDictionary.TryGetValue(id, out AssetReferenceGameObject assetReference))
                return assetReference;
            
            throw new KeyNotFoundException($"[WindowsConfig] AssetReference with Window id: {id} was not found.");
        }
    }
}
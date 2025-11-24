using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.Factory.UIFactory
{
    public class UIFactory : IUIFactory
    {
        private readonly DiContainer _container;
        private readonly Transform _uiParent;

        public UIFactory(DiContainer container, Transform uiParent)
        {
            _container = container;
            _uiParent = uiParent;
        }
        
        public GameObject CreateHudLayer(GameObject prefab) => 
            _container.InstantiatePrefab(prefab, _uiParent);

        public GameObject CreatePopUpLayer(GameObject prefab) => 
            _container.InstantiatePrefab(prefab, _uiParent);
    }
}
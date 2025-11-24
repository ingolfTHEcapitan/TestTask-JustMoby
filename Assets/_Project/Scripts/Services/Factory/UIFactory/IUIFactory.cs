using UnityEngine;

namespace _Project.Scripts.Services.Factory.UIFactory
{
    public interface IUIFactory
    {
        GameObject CreateHudLayer(GameObject prefab);
        GameObject CreatePopUpLayer(GameObject prefab);
    }
}
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factory.UIFactory
{
    public interface IUIFactory
    {
        GameObject CreateHudLayer(GameObject prefab);
        GameObject CreatePopUpLayer(GameObject prefab);
    }
}
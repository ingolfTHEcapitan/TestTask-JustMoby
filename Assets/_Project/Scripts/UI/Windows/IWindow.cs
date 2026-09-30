using System;
using UnityEngine;

namespace _Project.Scripts.UI.Windows
{
    public interface IWindow
    {
        event Action OnWindowDestroy;
    }
}
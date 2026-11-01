using System;
using UnityEngine;

namespace _Project.Scripts.UI.Windows
{
    public abstract class WindowBase : MonoBehaviour, IWindow
    {
        public event Action OnWindowDestroy;

        public abstract void Open();
        public abstract void Close();
    }
}
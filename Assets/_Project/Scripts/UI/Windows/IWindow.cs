using System;

namespace _Project.Scripts.UI.Windows
{
    public interface IWindow
    {
        event Action OnWindowDestroy;
    }
}
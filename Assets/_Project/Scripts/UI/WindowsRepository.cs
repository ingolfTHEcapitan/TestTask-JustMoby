using System;
using System.Collections.Generic;
using System.Data;
using _Project.Scripts.UI.Windows;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class WindowsRepository
    {
        private readonly Dictionary<Type, IWindow> _windowViews = new Dictionary<Type, IWindow>();
        
        public void Register<TWindow>(TWindow window) where TWindow : Component, IWindow => 
            _windowViews.Add(typeof(TWindow), window);

        public void UnRegister<TWindow>() where TWindow : Component, IWindow => 
            _windowViews.Remove(typeof(TWindow));

        public TWindow Get<TWindow>() where TWindow : Component, IWindow
        {
            Type viewType = typeof(TWindow);
            
            if (_windowViews.TryGetValue(viewType, out IWindow windowView))
                return (TWindow)windowView;
            
            throw new InvalidConstraintException(
                $"[WindowsRepository] {viewType.Name} requested before creation. Ensure it created before presenter resolution");
        }
        
        public bool TryGet<TWindow>(out TWindow window) where TWindow : Component, IWindow
        {
            bool result = _windowViews.TryGetValue(typeof(TWindow), out IWindow windowView);
            window = (TWindow)windowView;
            return result;
        }
    }
}
using UnityEngine;

namespace _Project.Scripts.UI.Common
{
    public class CursorController
    {
        public void SetCursorVisible(bool visible)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }
    }
}
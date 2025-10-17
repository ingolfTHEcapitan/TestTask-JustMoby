using UnityEngine;

namespace _Project._Scripts.Logic
{
    public static class CursorController
    {
        public static void SetCursorVisible(bool visible)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }
    }
}
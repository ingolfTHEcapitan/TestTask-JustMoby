using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.PlayerInput
{
    public class DesktopInputService : IInputService
    {
        public Vector2 GetMovementAxis() => 
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        public Vector2 GetLookAxis() => 
            new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        public bool IsFireButtonPressed() => 
            Input.GetButton("Fire1");
        
        public bool IsOpenStatsButtonPressed() => 
            Input.GetKeyDown(KeyCode.Tab);
    }
}
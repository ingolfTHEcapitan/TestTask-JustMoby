using UnityEngine;

namespace _Project._Scripts.Infrastructure.Services.PlayerInput
{
    public interface IInputService
    {
        Vector2 GetMovementAxis();
        Vector2 GetLookAxis();
        bool IsFireButtonPressed();
    }
}
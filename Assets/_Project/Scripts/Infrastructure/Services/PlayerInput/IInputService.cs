using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.PlayerInput
{
    public interface IInputService
    {
        Vector2 GetMovementAxis();
        Vector2 GetLookAxis();
        bool IsFireButtonPressed();
        bool IsOpenStatsButtonPressed();
    }
}
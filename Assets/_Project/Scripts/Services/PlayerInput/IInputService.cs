using System;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.PlayerInput
{
    public interface IInputService : ITickable
    {
        event Action OnOpenStatsButtonPressed;
        event Action OnMainMenuButtonPressed;
        
        Vector2 GetMovementAxis();
        Vector2 GetLookAxis();
        bool IsFireButtonPressed();
        bool IsOpenStatsButtonPressed();
        bool IsMainMenuButtonPressed();
    }
}
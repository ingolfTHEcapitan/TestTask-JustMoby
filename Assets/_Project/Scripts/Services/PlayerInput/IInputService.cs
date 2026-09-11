using System;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.PlayerInput
{
    public interface IInputService : ITickable
    {
        event Action OpenStatsButtonPressed;
        event Action MainMenuButtonPressed;
        
        Vector2 GetMovementAxis();
        Vector2 GetLookAxis();
        bool IsFireButtonPressed();
        bool IsOpenStatsButtonPressed();
        bool IsMainMenuButtonPressed();
    }
}
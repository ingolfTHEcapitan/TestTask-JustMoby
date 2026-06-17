using _Project.Scripts.Logic.Player.PlayerStats;
using _Project.Scripts.Logic.Player.PlayerStats.Data;
using _Project.Scripts.Services.GamePause;
using _Project.Scripts.Services.PlayerInput;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Player
{
    public class PlayerMovement: MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [Header("Settings")]
        [SerializeField] private float _gravity = 21f;

        private Vector3 _movementDirection;
        private PlayerStatsData _playerStatsData;
        private IGamePauseService _pauseService;
        private IInputService _inputService;

        private float Speed => _playerStatsData.GetStatValue(StatName.Speed);
        
        [Inject]
        private void Construct(PlayerStatsData playerStatsData, IGamePauseService pauseService, IInputService inputService)
        {
            _playerStatsData = playerStatsData;
            _pauseService = pauseService;
            _inputService = inputService;
        }
        
        private void Update()
        {
            if (_pauseService.IsPaused)
                return;

            Vector2 movementInput = _inputService.GetMovementAxis();
            Move(movementInput);
        }

        private void Move(Vector2 axis)
        {
            _movementDirection = new Vector3(axis.x * Speed, _movementDirection.y, axis.y * Speed);
            _movementDirection = transform.TransformDirection(_movementDirection);
            
            ApplyGravity();
            
            _characterController.Move(_movementDirection * Time.deltaTime);
        }

        private void ApplyGravity()
        {
            if (_characterController.isGrounded) 
                _movementDirection.y = -0.5f;
            else
                _movementDirection.y -= _gravity * Time.deltaTime;
        }
    }
}
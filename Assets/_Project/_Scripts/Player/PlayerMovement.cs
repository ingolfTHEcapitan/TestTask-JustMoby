using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.PlayerInput;
using _Project._Scripts.Logic.StatSystem;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerMovement: MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField, Space] private float _gravity = 21f;

        private Vector3 _movementDirection;
        private PlayerStatsSystem _playerStatsSystem;
        private IGamePauseService _pauseService;
        private IInputService _inputService;

        private float Speed => _playerStatsSystem.GetStatValue(StatName.Speed);
        
        public void Construct(PlayerStatsSystem playerStats, IGamePauseService pauseService, IInputService inputService)
        {
            _playerStatsSystem = playerStats;
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
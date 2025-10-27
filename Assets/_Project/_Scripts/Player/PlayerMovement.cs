using _Project._Scripts.Infrastructure.Services.GamePause;
using _Project._Scripts.Infrastructure.Services.PlayerInput;
using _Project._Scripts.Logic.PlayerStats;
using UnityEngine;
using Zenject;

namespace _Project._Scripts.Player
{
    public class PlayerMovement: MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField, Space] private float _gravity = 21f;

        private Vector3 _movementDirection;
        private PlayerStatsModel _playerStatsModel;
        private IGamePauseService _pauseService;
        private IInputService _inputService;

        private float Speed => _playerStatsModel.GetStatValue(StatName.Speed);
        
        [Inject]
        public void Construct(PlayerStatsModel playerStats, IGamePauseService pauseService, IInputService inputService)
        {
            _playerStatsModel = playerStats;
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
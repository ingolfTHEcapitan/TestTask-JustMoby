using _Project._Scripts.Player.StatSystem;
using _Project._Scripts.Services.GamePause;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerMovement: MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        
        [Header("Settings")]
        [SerializeField] private float _gravity = 21f;

        private Vector3 _movementDirection;
        private PlayerStatsSystem _playerStatsSystem;
        private IGamePauseService _pauseService;

        private float Speed => _playerStatsSystem.GetStatValue(StatName.Speed);
        
        public void Construct(PlayerStatsSystem playerStats, IGamePauseService pauseService)
        {
            _playerStatsSystem = playerStats;
            _pauseService = pauseService;
        }
        
        private void Update()
        {
            if (_pauseService.IsPaused)
                return;
            
            _movementDirection = new Vector3(Input.GetAxis("Horizontal") * Speed, _movementDirection.y, Input.GetAxis("Vertical") * Speed);
            _movementDirection = transform.TransformDirection(_movementDirection);
            

            if (_characterController.isGrounded) 
                _movementDirection.y = -0.5f;
            else
                _movementDirection.y -= _gravity * Time.deltaTime;

            _characterController.Move(_movementDirection * Time.deltaTime);
        }
    }
}
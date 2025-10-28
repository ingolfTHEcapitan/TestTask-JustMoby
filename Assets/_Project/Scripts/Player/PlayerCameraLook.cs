using _Project.Scripts.Infrastructure.Services.GamePause;
using _Project.Scripts.Infrastructure.Services.PlayerInput;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Player
{
    public class PlayerCameraLook : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CharacterController _characterController;
        
        [SerializeField, Space] private float _sensitivity;
        [SerializeField] private float _verticalRotationLimit;

        private Transform _cameraTransform;
        private Transform _playerTransform;
        private float _verticalRotation;
        private IGamePauseService _pauseService;
        private IInputService _inputService;

        [Inject]
        public void Construct(IGamePauseService pauseService, IInputService inputService)
        {
            _pauseService = pauseService;
            _inputService = inputService;
        }

        private void Start()
        {
            _cameraTransform = _camera.transform;
            _playerTransform = _characterController.transform;
        }
        
        private void Update()
        {
            if (_pauseService.IsPaused)
                return;
            
            Vector2 lookInput = _inputService.GetLookAxis();
            RotateCamera(lookInput);
        }

        private void RotateCamera(Vector2 axis)
        {
            _playerTransform.Rotate(0, axis.x * _sensitivity, 0);
            
            _verticalRotation -= axis.y * _sensitivity;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -_verticalRotationLimit, _verticalRotationLimit);
            _cameraTransform.localEulerAngles = new Vector3(_verticalRotation, 0, 0);
        }
    }
}
using _Project._Scripts.Services.GamePause;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerCameraLook : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CharacterController _characterController;
        
        [Header("Settings")]
        [SerializeField] private float _sensitivity;
        [SerializeField] private float _verticalRotationLimit;

        private Transform _cameraTransform;

        private float _verticalRotation;
        private Transform _playerTransform;
        private IGamePauseService _pauseService;

        public void Construct(IGamePauseService pauseService) => 
            _pauseService = pauseService;
        
        private void Start()
        {
            _cameraTransform = _camera.transform;
            _playerTransform = _characterController.transform;
        }
        
        private void Update()
        {
            if (_pauseService.IsPaused)
                return;
            
            _playerTransform.Rotate(0, Input.GetAxis("Mouse X") * _sensitivity, 0);
            
            _verticalRotation -= Input.GetAxis("Mouse Y") * _sensitivity;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -_verticalRotationLimit, _verticalRotationLimit);
            _cameraTransform.localEulerAngles = new Vector3(_verticalRotation, 0, 0);
        }
    }
}
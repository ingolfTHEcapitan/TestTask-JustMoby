using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerMovement: MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        
        [Header("Settings")]
        [SerializeField] private float _speed = 6f;
        [SerializeField] private float _gravity = 21f;

        private Vector3 _movementDirection;

        private void Update()
        {
            _movementDirection = new Vector3(Input.GetAxis("Horizontal") * _speed, _movementDirection.y, Input.GetAxis("Vertical") * _speed);
            _movementDirection = transform.TransformDirection(_movementDirection);
            

            if (_characterController.isGrounded) 
                _movementDirection.y = -0.5f;
            else
                _movementDirection.y -= _gravity * Time.deltaTime;

            _characterController.Move(_movementDirection * Time.deltaTime);
        }
    }
}
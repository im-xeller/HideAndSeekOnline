using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Camera Settings")] 
        [SerializeField] private Camera cam;
        [SerializeField] private Transform camTarget;
        [SerializeField] private float sensitivityX;
        [SerializeField] private float sensitivityY;
        [SerializeField] private float topClamp;
        [SerializeField] private float bottomClamp;

        [Header("Movement Settings")] 
        [SerializeField] private LayerMask ground;
        [SerializeField] private Transform feet;

        private PlayerInput _playerInput;
        
        // Camera variables
        private float _camTargetX;
        private float _camTargetY;

        // Movement variables
        private CharacterController _character;

        private Vector3 _targetDirection;
        private float _speed = 10f;
        private bool _isIOnGround;
        private float _verticalVelocity;
        private const float Gravity = -9.81f;
        private const float GroundOffset = 0.1f;
        private const float JumpStrength = 2f;

        private void OnEnable()
        {
            _playerInput = GetComponent<PlayerInput>();
            _character = GetComponent<CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            
            HandlePlayerMovementAndRotation();
            HandleJump();
            HandleCameraRotation();
            HandleGravity();

            _character.Move(_targetDirection.normalized * _speed * Time.deltaTime +
                            new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);
        }

        private void HandlePlayerMovementAndRotation()
        {
            Vector2 input = _playerInput.actions["Move"].ReadValue<Vector2>();

            if (!(input.magnitude > 0))
            {
                _targetDirection = Vector3.zero;
                return;
            }

            Vector3 xDir = cam.transform.right * input.x;
            Vector3 zDir = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f) * Vector3.forward * input.y;
            
            _targetDirection = xDir + zDir;
        }

        private void HandleJump()
        {
            if (_playerInput.actions["Jump"].phase != InputActionPhase.Performed || !_isIOnGround) return;

            _verticalVelocity = Mathf.Sqrt(JumpStrength * -2f * Gravity);
        }

        private void HandleCameraRotation()
        {
            Vector2 input = _playerInput.actions["Look"].ReadValue<Vector2>();
            
            _camTargetX += input.x * sensitivityX * Time.deltaTime;
            _camTargetY += input.y * sensitivityY * Time.deltaTime * -1;
                
            _camTargetY = Mathf.Clamp(_camTargetY, bottomClamp, topClamp);
                
            camTarget.transform.rotation = Quaternion.Euler(_camTargetY, _camTargetX, 0f);
        }

        private void HandleGravity()
        {
            _verticalVelocity += Gravity * Time.deltaTime;

            _isIOnGround = Physics.CheckSphere(feet.position, GroundOffset, ground);

            if (_isIOnGround && _verticalVelocity <= 0f)
            {
                _verticalVelocity = -2f;
            }
        }
    }
}

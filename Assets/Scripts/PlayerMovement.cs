using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    /*Setting up Player Settings*/
    [Header("Player Reference")]
    [SerializeField]
    private Transform _playerGroundPosition;
    [SerializeField]
    private Transform _cameraTransform;

    [Header("Move/Sprint Settings")]
    [SerializeField]
    private float _speed = 10.0f;
    [SerializeField]
    private float _sprintSpeed;
    [SerializeField]
    private float _sprintRecharge = 1f;
    [SerializeField]
    private float _sprintDuration = 10f;
    [SerializeField]
    private float _acceleration = 60f;
    [SerializeField]
    private float _deceleration = 80f;
    
    [Header("Ground Settings")]
    [SerializeField]
    private float _groundDistance = 0.2f;
    [SerializeField]
    private LayerMask _groundLayer;

    [Header("Jump/Gravity Settings")]
    [SerializeField]    
    private float _jumpSpeed;
    [SerializeField]
    private float _baseGravity;
    [SerializeField]
    private float _earlyJumpGravity;
    [SerializeField]
    private float _fallingGravity;
    [SerializeField]
    private float _fallSpeedCap;

    [Header("Dash Settings")]
    [SerializeField]
    private float _dashSpeed;
    [SerializeField]
    private float _dashLength;
    [SerializeField]
    private float _dashCooldown;


    private Rigidbody _rb;
    private PlayerInputManager _playerInputManager;

    private Vector3 _movement;
    private bool _isGrounded;
    private bool _isSprinting;
    private float _currentSprintTime;

    private bool _jumpTriggered;
    private bool _longJump;

    private bool _isDashing;
    private bool _playerAbleToDash = true;

    private void Awake()
    {
        //grabs rigidbody and input manager component
        _rb = GetComponent<Rigidbody>();
        _playerInputManager = GetComponent<PlayerInputManager>();

        //sets sprint duration to full
        _currentSprintTime =  _sprintDuration;
    }
    //when the components are in use
    private void OnEnable()
    {
        //sets dash and jump to be active when they are pressed
        _playerInputManager.OnJumpButtonPressed += JumpButtonPressed;
        _playerInputManager.OnJumpButtonReleased += JumpButtonReleased;
        _playerInputManager.OnDashButtonPressed += DashButtonPressed;
    }
    //when the components are not in use
    private void OnDisable()
    {
        //sets dash and jump to be inactive when they are not pressed
        _playerInputManager.OnJumpButtonPressed -= JumpButtonPressed;
        _playerInputManager.OnJumpButtonReleased -= JumpButtonReleased;
        _playerInputManager.OnDashButtonPressed -= DashButtonPressed;
    }

    // Update is called once per frame
    private void Update()
    {
        PlayerMoveDirection();
    }

    //runs at a consistent fixed time instead of per frame
    private void FixedUpdate()
    {
        
        if (_isDashing) return;

        GroundCheck();
        SprintUpdate();
        RotateCharacter();
        MoveCharacter(_movement);
        PlayerJump();
        JumpPhysics();
    }

    private void PlayerMoveDirection()
    {
        Vector2 inputVector = _playerInputManager.MovementInput;

        Vector3 forwardCam = _cameraTransform.forward;
        Vector3 rightCam = _cameraTransform.right;

        forwardCam.y = 0f;
        rightCam.y = 0f;

        forwardCam.Normalize();
        rightCam.Normalize();
        _movement = (forwardCam * inputVector.y) + (rightCam * inputVector.x);
    }

    private void GroundCheck()
    {
        _isGrounded = Physics.CheckSphere(_playerGroundPosition.position, _groundDistance, _groundLayer);
    }

    private void SprintUpdate()
    {
        bool playerMoving = _movement.sqrMagnitude > 0.001f;
        if (_isGrounded && _playerInputManager.PlayerIsSprinting && playerMoving && _currentSprintTime > 0f)
        {
            _isSprinting = true;
            _currentSprintTime -= Time.fixedDeltaTime;

            if (_currentSprintTime < 0f)
            {
                _currentSprintTime = 0f;
            }
        }
        else
        {
            _isSprinting = false;

            if(!_playerInputManager.PlayerIsSprinting && _currentSprintTime < _sprintDuration)
            {
                _currentSprintTime += _sprintRecharge * Time.fixedDeltaTime;

                if(_currentSprintTime > _sprintDuration)
                {
                    _currentSprintTime = _sprintDuration;
                }

                Debug.Log($"Recharging sprint: {_currentSprintTime:F2}");
            }
        }
    }
    private void MoveCharacter(Vector3 direction)
    {
        float targetSpeed = _isSprinting ? _sprintSpeed : _speed;
        Vector3 targetVelocity = _movement * targetSpeed;

        Vector3 currentVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

        bool movementInputTrue = _movement.sqrMagnitude > 0.001f;
        float accelerationRate = movementInputTrue ? _acceleration : _deceleration;

        Vector3 smoothVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, accelerationRate * Time.fixedDeltaTime);
        _rb.linearVelocity = new Vector3(smoothVelocity.x, _rb.linearVelocity.y, smoothVelocity.z);
    }
    private void RotateCharacter()
    {
        float cameraYaw = _cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, cameraYaw, 0f);
        _rb.MoveRotation(targetRotation);
    }

    private void JumpButtonPressed()
    {
        _longJump = true;
        if (_isGrounded)
        {
            _jumpTriggered = true;
        }
    }

    private void JumpButtonReleased()
    {
        _longJump = false;
    }

    private void PlayerJump()
    {
        if (!_jumpTriggered) return;

        Vector3 currentVelocity = _rb.linearVelocity;
        currentVelocity.y = _jumpSpeed;
        _rb.linearVelocity = currentVelocity;

        _jumpTriggered = false;
    }
    private void JumpPhysics()
    {
        if (_isGrounded && _rb.linearVelocity.y <= 0f) return;

        Vector3 velocity =  _rb.linearVelocity;
        float gravityMultiplier = 1f;

        if (velocity.y < 0f)
        {
            gravityMultiplier = _fallingGravity;
        }
        else if (velocity.y > 0f && !_longJump)
        {
            gravityMultiplier = _earlyJumpGravity;
        }
        else if (velocity.y > 0f)
        {
            gravityMultiplier = _baseGravity;
        }
        
        velocity.y += Physics.gravity.y * (gravityMultiplier - 1f) * Time.fixedDeltaTime;

        if(_fallSpeedCap >0f && velocity.y < -_fallSpeedCap)
        {
            velocity.y = -_fallSpeedCap;
        }
        _rb.linearVelocity = velocity;
    }

    private void DashButtonPressed()
    {
        if (_playerAbleToDash && !_isDashing)
        {
            StartCoroutine(PlayerDash());
            print("Player Dashed");
        }
    }
    private IEnumerator PlayerDash()
    {
        _playerAbleToDash = false;
        _isDashing = true;

        Vector3 dashDirection = _movement.sqrMagnitude > 0.01f ? _movement.normalized : transform.forward;

        _rb.linearVelocity = dashDirection * _dashSpeed;
        yield return new WaitForSeconds(_dashLength);

        _isDashing = false;

        yield return new WaitForSeconds(_dashCooldown);
        _playerAbleToDash = true;
    }   
    
}

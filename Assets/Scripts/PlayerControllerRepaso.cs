using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerRepaso : MonoBehaviour
{
    Animator _animator;
    CharacterController _controller;

    InputAction _jumpAction;
    InputAction _moveAction;
    Vector2 _moveValue;

    [SerializeField] float _movementSpeed = 5;
    [SerializeField] float _jumpHeight = 2;
    [SerializeField] float _gravity = -9.81f;

    [SerializeField] Transform _sensorPosition;
    [SerializeField] float _sensorRadius;
    [SerializeField] LayerMask _groundLayer;

    [SerializeField] Vector3 playerGravity;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        _jumpAction = InputSystem.actions["Jump"];
        _moveAction = InputSystem.actions["Move"];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _moveValue = _moveAction.ReadValue<Vector2>();

        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        Movement();

        Gravity();
    }

    void Movement()
    {
        Vector3 moveDirection = new Vector3(_moveValue.x, 0, _moveValue.y);

        _controller.Move(moveDirection * _movementSpeed * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);
    }

    void Jump()
    {
        playerGravity.y = Mathf.Sqrt(_jumpHeight * _gravity * -2);
        _controller.Move(playerGravity * Time.deltaTime);
    }

    void Gravity()
    {
        if(!IsGrounded())
        {
            playerGravity.y += _gravity * Time.deltaTime;
        }
        else if(IsGrounded() && playerGravity.y < 0)
        {
            playerGravity.y = -2;
        }

        _controller.Move(playerGravity * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_sensorPosition.position, _sensorRadius);
    }
}

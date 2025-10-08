using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Componentes
    private CharacterController _controller;

    //Inputs
    private InputAction _moveAction;
    private Vector2 _moveInput;
    private InputAction _jumpAction;

    private InputAction _lookAction;
    private Vector2 _lookInput;

    private InputAction _aimAction;


    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _jumpHeight = 2;

    [SerializeField] private float _smoothTime = 0.2f;
    private float _turnSmoothVelocity;

    //Gravedad
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private Vector3 _playerGravity;

    //Ground Sensor
    [SerializeField] Transform _sensor;
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] float _sensorRadius;

    public Transform _mainCamera;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _lookAction = InputSystem.actions["Look"];
        _aimAction = InputSystem.actions["Aim"];

        _mainCamera = Camera.main.transform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookInput = _lookAction.ReadValue<Vector2>();

        //MovimientoCutre();
        //Movimiento2();

        if (_aimAction.IsInProgress())
        {
            AimMovement();
        }
        else
        {
            Movement();
        }      

        if (_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        Gravity();
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _controller.Move(moveDirection.normalized * _movementSpeed * Time.deltaTime);
        }
    }

    void AimMovement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);
        
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamera.eulerAngles.y, ref _turnSmoothVelocity, _smoothTime);

        transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

        if (direction != Vector3.zero)
        {
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _controller.Move(moveDirection.normalized * _movementSpeed * Time.deltaTime);
        }
    }

    void Movimiento2()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        Ray ray = Camera.main.ScreenPointToRay(_lookInput);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 playerForward = hit.point - transform.position;
            Debug.Log(hit.transform.name);
            playerForward.y = 0;
            transform.forward = playerForward;
        }

        if (direction != Vector3.zero)
            {
                _controller.Move(direction.normalized * _movementSpeed * Time.deltaTime);
            }
    }

    void MovimientoCutre()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            _controller.Move(direction.normalized * _movementSpeed * Time.deltaTime);
        }
    }

    void Jump()
    {
        _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);

        _controller.Move(_playerGravity * Time.deltaTime);
    }

    void Gravity()
    {
        if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if (IsGrounded() && _playerGravity.y < _gravity)
        {
            _playerGravity.y = _gravity;
        }
        

        _controller.Move(_playerGravity * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensor.position, _sensorRadius, _groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_sensor.position, _sensorRadius);
    }
}

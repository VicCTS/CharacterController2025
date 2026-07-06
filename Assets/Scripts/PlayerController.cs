using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Componentes
    private CharacterController _controller;
    [SerializeField]private Animator _animator;

    //Inputs
    private InputAction _moveAction;
    private Vector2 _moveInput;
    private InputAction _jumpAction;

    private InputAction _lookAction;
    private Vector2 _lookInput;

    private InputAction _aimAction;
    private InputAction _grabAction;
    private InputAction _throwAction;


    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _jumpHeight = 2;
    [SerializeField] private float _pushForce = 10;
    [SerializeField] private float _throwForce = 20;

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

    [SerializeField] private Vector3 _handsSensorSize;
    [SerializeField] private Transform _hands;
    [SerializeField] private Transform _grabedObject;

    [SerializeField] float _speedChangeRate = 10;
    float _speed;
    float _animationSpeed;

    float targetAngle;

    bool isSprinting = false;
    float _sprintSpeed = 8;

    public float jumpTimeOut = 0.5f;
    public float fallTimeOut = 0.15f;

    float _jumpTimeOutDelta;
    float _fallTimeOutDelta;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _lookAction = InputSystem.actions["Look"];
        _aimAction = InputSystem.actions["Aim"];
        _grabAction = InputSystem.actions["Interact"];
        _throwAction = InputSystem.actions["Throw"];

        _mainCamera = Camera.main.transform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _jumpTimeOutDelta = jumpTimeOut;
        _fallTimeOutDelta = fallTimeOut;
    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookInput = _lookAction.ReadValue<Vector2>();

        //MovimientoCutre();
        //Movimiento2();

        Gravity();

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

        
        

        if (_aimAction.WasPerformedThisFrame())
        {
            Attack();
        }

        if (_grabAction.WasPerformedThisFrame())
        {
            GrabObject();
        }

        if (_throwAction.WasPerformedThisFrame())
        {
            //Throw();
            RayTest();
        }

        
    }
    
    void Attack()
    {
        Ray ray = Camera.main.ScreenPointToRay(_lookInput);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            IDamageable damageable = hit.transform.GetComponent<IDamageable>();

            if(damageable != null)
            {
                damageable.TakeDamage(5);
            }
        }
    }


    

    void Movement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        /*float targetSpeed;

        if(isSprinting)
        {
            targetSpeed = _sprintSpeed;
        }
        else
        {
            targetSpeed = _movementSpeed;
        }*/

        float targetSpeed = _movementSpeed;

        if(direction == Vector3.zero)
        {
            targetSpeed = 0;
        }

        float currentSpeed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        if(currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentSpeed, targetSpeed * direction.magnitude, Time.deltaTime * _speedChangeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }


        _animationSpeed = Mathf.Lerp(_animationSpeed, targetSpeed, Time.deltaTime * _speedChangeRate);

        if(_animationSpeed < 0.1f)
        {
            _animationSpeed = 0;
        }

        _animator.SetFloat("Speed", _animationSpeed);

        //_animator.SetFloat("Vertical", direction.magnitude);
        //_animator.SetFloat("Horizontal", 0);

        /*if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _controller.Move(moveDirection.normalized * _movementSpeed * Time.deltaTime);
        }*/

        if (direction != Vector3.zero)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        }

        Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

        _controller.Move(_speed * Time.deltaTime * moveDirection.normalized + _playerGravity * Time.deltaTime);
    }

    void AimMovement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        _animator.SetFloat("Horizontal", _moveInput.x, 0.2f, Time.deltaTime);
        _animator.SetFloat("Vertical", _moveInput.y, 0.2f, Time.deltaTime);
        
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
        if(_jumpTimeOutDelta <= 0)
        {
            _animator.SetBool("Jump", true);

            _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
        }
        //_animator.SetBool("IsJumping", true);
        

        //_controller.Move(_playerGravity * Time.deltaTime);
    }

    void Gravity()
    {
        /*if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if (IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = _gravity;
            _animator.SetBool("IsJumping", false);
        }*/

        //_controller.Move(_playerGravity * Time.deltaTime);

        _animator.SetBool("Grounded", IsGrounded());

        

        if(IsGrounded())
        {
            _fallTimeOutDelta = fallTimeOut;

            _animator.SetBool("Jump", false);
            _animator.SetBool("Fall", false);

            if(_playerGravity.y < 0)
            {
                _playerGravity.y = -2;
            }

            if(_jumpTimeOutDelta >= 0)
            {
                _jumpTimeOutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeOutDelta = jumpTimeOut;

            if(_fallTimeOutDelta >= 0)
            {
                _fallTimeOutDelta -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool("Fall", true);
            }

            
            _playerGravity.y += _gravity * Time.deltaTime;
        }
    }

    /*bool IsGrounded()
    {
        return Physics.CheckSphere(_sensor.position, _sensorRadius, _groundLayer);
    }*/
    
    bool IsGrounded()
    {
        if (Physics.Raycast(_sensor.position, -transform.up, _sensorRadius, _groundLayer))
        {
            Debug.DrawRay(_sensor.position, -transform.up * _sensorRadius, Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(_sensor.position, -transform.up * _sensorRadius, Color.green);
            return false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_sensor.position, _sensorRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_hands.position, _handsSensorSize);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.gameObject.tag == "Empujable")
        {
            Rigidbody rBody = hit.collider.attachedRigidbody;
            //Rigidbody rBody = hit.transform.GetComponent<Rigidbody>();

            if (rBody == null || rBody.isKinematic)
            {
                return;
            }

            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            rBody.linearVelocity = pushDirection * _pushForce / rBody.mass;
        }
    }

    void GrabObject()
    {
        if (_grabedObject == null)
        {
            Collider[] objectsToGrab = Physics.OverlapBox(_hands.position, _handsSensorSize);

            foreach (Collider item in objectsToGrab)
            {
                IGrabeable grabeableObject = item.GetComponent<IGrabeable>();

                if (grabeableObject != null)
                {
                    _grabedObject = item.transform;
                    _grabedObject.SetParent(_hands);
                    _grabedObject.position = _hands.position;
                    _grabedObject.rotation = _hands.rotation;
                    _grabedObject.GetComponent<Rigidbody>().isKinematic = true;

                    return;
                }
            }
        }
        else
        {
            _grabedObject.SetParent(null);
            _grabedObject.GetComponent<Rigidbody>().isKinematic = false;
            _grabedObject = null;
        }

    }

    void Throw()
    {
        if (_grabedObject == null)
        {
            return;
        }

        Rigidbody grabedBody = _grabedObject.GetComponent<Rigidbody>();

        _grabedObject.SetParent(null);
        grabedBody.isKinematic = false;
        grabedBody.AddForce(_mainCamera.transform.forward * _throwForce, ForceMode.Impulse);
        _grabedObject = null;
    }
    
    void RayTest()
    {
        //Raycast simple
        if (Physics.Raycast(transform.position, transform.forward, 5))
        {
            Debug.Log("hit");
            Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        }


        //Raycast "Avanzado"
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5))
        {
            Debug.Log(hit.transform.name);
            Debug.Log(hit.transform.position);
            Debug.Log(hit.transform.gameObject.layer);
            Debug.Log(hit.transform.tag);

            /*if (hit.transform.tag == "Empujable")
            {
                Box box = hit.transform.GetComponent<Box>();

                if (box != null)
                {
                    Debug.Log("cosas");
                }
            }*/

            IDamageable damageable = hit.transform.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(5);
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(_lookInput);
        RaycastHit hit2;
        if (Physics.Raycast(ray, out hit2, Mathf.Infinity))
        {
            Vector3 playerForward = hit2.point - transform.position;
            Debug.Log(hit.transform.name);
            playerForward.y = 0;
            transform.forward = playerForward;
        }
    }
}
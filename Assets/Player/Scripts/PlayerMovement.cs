using Helper.Libs;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Vector2 _rawMovementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _smoothingVelocity = Vector2.zero;
    private Vector2 _rotationInput;

    private float _fallingSpeed;

    private Transform _camera;
    private Transform _model;
    private CharacterController _controller;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float inputSmoothingTime;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravity;
    [SerializeField] private float maxFallingSpeed;

    [Header("Camera Settings")]
    [SerializeField][Range(0.1f, 2)] private float horizontalSensitivity;
    [SerializeField][Range(0.1f, 2)] private float verticalSensitivity;

    private void Awake()
    {
        _camera = transform.Find("Camera");
        if(_camera == null)
        {
            Debug.LogError("Error! PlayerMovement could not find child with name Camera");
        }

        _model = transform.Find("Model");
        if(_model == null)
        {
            Debug.LogError("Error! Playermovement could not find child with name Model");
        }

        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        SmootheInput();
        Move();
        Gravity();
        Rotate();
        if(_rawMovementInput != Vector2.zero) RotateModelTowardsMovementDirection();
    }

    //move the player based on input
    private void Move()
    {
        Vector3 movement = (_smoothedMovementInput.y * transform.forward + _smoothedMovementInput.x * transform.right) * movementSpeed * Time.deltaTime;
        movement.y = _fallingSpeed * Time.deltaTime;
        _controller.Move(movement);
    }

    //rotate the player based on input
    private void Rotate()
    {
        Vector3 rotation = new Vector3(_rotationInput.y * verticalSensitivity, _rotationInput.x * horizontalSensitivity, 0);
        _camera.Rotate(rotation, Space.Self);

        //clamp the vertical rotation so you can't make the camera do weird things
        float angleX = MathExtra.ClampAngle(_camera.eulerAngles.x, -80, 80);

        _camera.eulerAngles = new Vector3(angleX, _camera.eulerAngles.y, 0);

        //if the playyer is inputting a move, rotate the player so it's facing the same direction as the camera, and put the camera behind the player
        if(_rawMovementInput != Vector2.zero)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _camera.eulerAngles.y, transform.eulerAngles.z);
            _camera.localEulerAngles = new Vector3(_camera.eulerAngles.x, _camera.eulerAngles.z);
        }
    }

    private void Gravity()
    {
        //set fallingspeed to a low number instead of zero for better reliability
        if (_controller.isGrounded)
        {
            _fallingSpeed = -0.1f;
            return;
        }

        _fallingSpeed -= gravity * Time.deltaTime;
        if(-_fallingSpeed > maxFallingSpeed) _fallingSpeed = -maxFallingSpeed;
    }

    //do a jump with a set height by setting the speed to an amount based on the strenght of gravity and the height of the jump
    private void Jump()
    {
        _fallingSpeed = Mathf.Sqrt(2 * gravity * jumpHeight);
    }

    //rotate the model in the direction the player is travelling
    private void RotateModelTowardsMovementDirection()
    {
        float angle = Vector3.SignedAngle(_smoothedMovementInput.y * transform.forward + _smoothedMovementInput.x * transform.right, transform.forward, Vector3.up);
        _model.localEulerAngles = new Vector3(_model.localEulerAngles.x, -angle, _model.localEulerAngles.z);
    }

    private void SmootheInput()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(_smoothedMovementInput, _rawMovementInput, ref _smoothingVelocity, inputSmoothingTime);
    }

    public void GetMovementInput(InputAction.CallbackContext context)
    {

        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _rawMovementInput = context.ReadValue<Vector2>();
                break;

            case InputActionPhase.Canceled:
                _rawMovementInput = Vector2.zero;
                break;

            default:
                break;
        }
    }

    public void GetRotationInput(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _rotationInput = context.ReadValue<Vector2>();
                break;

            case InputActionPhase.Canceled:
                _rotationInput = Vector2.zero;
                break;

            default:
                break;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Jump();
                break;

            default:
                break;
        }
    }
}

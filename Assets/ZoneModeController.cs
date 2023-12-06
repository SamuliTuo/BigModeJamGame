using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class ZoneModeController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float sprintSpeed = 2.0f;
    [SerializeField] private float speedChangeRate = 1.0f;
    [SerializeField] private float velocityChangeRate = 1.0f;

    [SerializeField] private float ascendAndDescend_maxSpeed = 1.0f;
    [SerializeField] private float ascendAndDescend_accelerate = 1.0f;
    [SerializeField] private float ascendAndDescend_decelerate = 1.0f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    private Animator _animator;
    private CharacterController _controller;
    private StarterAssetsInputs _input;
    private GameObject _mainCamera;

    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;


    void Start()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    
    bool bafsf = false;
    public void UpdateZoneMode()
    {
        print(transform.position);
        if (bafsf == false)
        {
            _animator.Play("ZoneMode");
            bafsf = true;
        }

        Move();
    }

    private void Move()
    {
        float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        Vector3 oldVelo = _controller.velocity * Time.deltaTime;
        Vector3 newVelo = Vector3.MoveTowards(oldVelo, targetDirection.normalized * (_speed * Time.deltaTime), velocityChangeRate) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;

        if (_input.ascend || _input.descend)
        {
            if (_input.ascend) 
                _verticalVelocity = Mathf.MoveTowards(_verticalVelocity, ascendAndDescend_maxSpeed, ascendAndDescend_accelerate * Time.deltaTime);
            if (_input.descend)
                _verticalVelocity = Mathf.MoveTowards(_verticalVelocity, -ascendAndDescend_maxSpeed, ascendAndDescend_accelerate * Time.deltaTime);
        }
        else
        {
            _verticalVelocity = Mathf.MoveTowards(_verticalVelocity, 0, ascendAndDescend_decelerate * Time.deltaTime);
        }

        _controller.Move(newVelo);//targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }
}

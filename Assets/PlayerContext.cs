using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContext
{
    private CharacterController _controller;
    private Camera _camera;
    private PlayerInput _playerInput;
    private Vector2 _mouseDelta;
    private Vector2 _moveDirection;
    private bool _isRunning = false;
    private float _verticalVelocity = 0f;
    private bool _isJumping = false;
    private bool _interactPressed = false;  // ← НОВОЕ: нажата ли F
    public GameObject PlayerModel { get; set; }
    public CharacterController Controller => _controller;
    public Camera Camera => _camera;
    public PlayerInput PlayerInput => _playerInput;
    public Vector2 MouseDelta => _mouseDelta;
    public Vector2 MoveDirection => _moveDirection;

    public bool IsInside;
    public bool IsRunning => _isRunning;
    public float VerticalVelocity { get => _verticalVelocity; set => _verticalVelocity = value; }
    public bool IsJumping => _isJumping;
    public bool InteractPressed => _interactPressed;  // ← НОВОЕ

    public Transform CeilingCheck;
    public LayerMask GroundMask;

    public PlayerContext(CharacterController controller, Camera camera, PlayerInput playerInput)
    {
        _controller = controller;
        _camera = camera;
        _playerInput = playerInput;

        // === Твои старые действия ===
        var rotation = playerInput.currentActionMap["Rotation"];
        rotation.performed += ctx => _mouseDelta = ctx.ReadValue<Vector2>();
        rotation.canceled += ctx => _mouseDelta = Vector2.zero;

        var move = playerInput.currentActionMap["Move"];
        move.performed += ctx => _moveDirection = ctx.ReadValue<Vector2>();
        move.canceled += ctx => _moveDirection = Vector2.zero;

        var run = playerInput.currentActionMap["Run"];
        run.performed += ctx => _isRunning = ctx.ReadValue<float>() > 0.5f;
        run.canceled += ctx => _isRunning = false;

        var jump = playerInput.currentActionMap["Jump"];
        jump.performed += ctx => _isJumping = true;
        jump.canceled += ctx => _isJumping = false;

        // === НОВОЕ: действие Interact (F) ===
        var interact = playerInput.currentActionMap.FindAction("Interact");
        if (interact != null)
        {
            interact.performed += ctx => _interactPressed = true;
            interact.canceled += ctx => _interactPressed = false;
        }
    }

    // Метод, чтобы сбросить нажатие F (чтобы не срабатывало дважды)
    public void ConsumeInteract()
    {
        _interactPressed = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class FirstPersonPlayerController : PlayerController
{
    private MovementBehaviour movementBehaviour;
    private PlayerInputActions inputActions;
    public PlayerInputActions InputActions => inputActions;

    public bool canJumpWhileCrouching = true;
    public bool canJump = true;

    private bool hasMovementBehaviour = true;

    public bool IsMoving => inputActions.PlayerMovement.Movement.ReadValue<Vector2>() != Vector2.zero;

    public event System.Action OnJump;
    public event System.Action<bool> OnCrouch;
    public event System.Action OnToggleCrouch;
    public event System.Action<bool> OnSprint;
    public event System.Action OnAttack;

    public static class Tags
    {
        public static readonly string PLAYER_WALKABLE = "PlayerWalkable";
    }

    private void Awake()
    {
        if (movementBehaviour == null)
        {
            if (TryGetComponent<MovementBehaviour>(out MovementBehaviour movementBehaviour))
            {
                this.movementBehaviour = movementBehaviour;
            }
            else
            {
                hasMovementBehaviour = false;
            }
        }

        if (GameManager.Instance.playerController == null)
        {
            GameManager.Instance.playerController = this;
        }

        if (GameManager.Instance.player == null)
        {
            GameManager.Instance.player = gameObject;
        }
    }

    private void SetupInputEvents()
    {
        inputActions.PlayerMovement.Jump.performed += ctx => OnJump?.Invoke();
        inputActions.PlayerMovement.Crouch.performed += ctx => OnCrouch?.Invoke(true);
        inputActions.PlayerMovement.Crouch.canceled += ctx => OnCrouch?.Invoke(false);
        inputActions.PlayerMovement.CrouchToggle.performed += ctx => OnToggleCrouch?.Invoke();
        inputActions.PlayerMovement.Sprint.performed += ctx => OnSprint?.Invoke(true);
        inputActions.PlayerMovement.Sprint.canceled += ctx => OnSprint?.Invoke(false);
        inputActions.PlayerMovement.Attack.performed += ctx => OnAttack?.Invoke();
    }

    private void SetupMovementInput()
    {
        inputActions.PlayerMovement.Jump.performed += Jump;
        inputActions.PlayerMovement.Crouch.performed += StartCrouch;
        inputActions.PlayerMovement.Crouch.canceled += StopCrouch;
        inputActions.PlayerMovement.CrouchToggle.performed += ToggleCrouch;
        inputActions.PlayerMovement.Sprint.performed += StartSprint;
        inputActions.PlayerMovement.Sprint.canceled += StopSprint;

        // use simple raycast attack if present
        if (TryGetComponent<SimpleRaycastAttack>(out SimpleRaycastAttack simpleRaycastAttack))
        {
            OnAttack += () => simpleRaycastAttack.Attack(Camera.main.transform.position, Camera.main.transform.forward);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!canJump)
        {
            return;
        }
        if (!canJumpWhileCrouching && movementBehaviour.IsCrouching)
        {
            return;
        }

        movementBehaviour.Jump();
    }

    private void StartSprint(InputAction.CallbackContext context)
    {
        movementBehaviour.isSprinting = true;
    }

    private void StopSprint(InputAction.CallbackContext context)
    {
        movementBehaviour.isSprinting = false;
    }

    private void StartCrouch(InputAction.CallbackContext context)
    {
        movementBehaviour.Crouch(true);
    }
    private void StopCrouch(InputAction.CallbackContext context)
    {
        movementBehaviour.Crouch(false);
    }

    private void ToggleCrouch(InputAction.CallbackContext context)
    {
        movementBehaviour.ToggleCrouch();
    }

    // Start is called before the first frame update
    void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        SetupInputEvents();

        if (hasMovementBehaviour)
        {
            SetupMovementInput();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movementInput = inputActions.PlayerMovement.Movement.ReadValue<Vector2>();
        movementBehaviour.Move(movementInput);
    }
}

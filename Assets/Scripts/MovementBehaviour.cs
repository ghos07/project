using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    private ModifierManager modifiers;
    [SerializeField] public Transform collision;
    public List<Tag> walkableTags = new List<Tag>();
    [SerializeField] private bool moveInFacingDirection = true;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float sprintMaxSpeedModifier = 1.5f;
    [SerializeField] private float sprintAccelerationModifier = 1.5f;
    [SerializeField] private float crouchSpeedModifier = 0.5f;
    [SerializeField] private float crouchAccelerationModifier = 0.9f;
    [SerializeField] private float crouchSize = 0.65f;
    [SerializeField] private float crouchSpeed = 1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float airAccelerationModifier = 0.5f;
    [SerializeField] private float airSpeedModifier = 1f;
    [SerializeField] private float defaultDrag = 1f;
    [SerializeField] private float airDrag = 1f;
    [HideInInspector] public Vector2 move;
    private float effectiveDrag;
    private Vector3 moveForce = Vector3.zero;
    private bool crouching = false;
    private Vector3 collisionScale;
    private float crouchTimer = -1f;
    private float previousCrouchSize = 1f;
    [HideInInspector] public bool isSprinting = false;
    [InspectorLabel("Grounded Collider")]
    [SerializeField] public float radius = 0.2f;
    [SerializeField] public float height = 0.62f;

    public bool IsCrouching => crouching;


    private bool isGrounded
    {
        get
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position - Vector3.up * height * collision.lossyScale.y, radius * collision.lossyScale.y);
            foreach (Collider collider in colliders)
            {
                if (TagManager.HasAnyTags(collider, walkableTags.ToArray()))
                {
                    return true;
                }
            }
            return false;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        effectiveDrag = defaultDrag;

        if (collision == null)
        {
            collision = transform;
            Debug.LogWarning("No collision transform set for PlayerMovement, defaulting to transform");
        }

        collisionScale = collision.localScale;
        previousCrouchSize = collision.localScale.y;

        modifiers = ModifierManager.GetModifierManager(gameObject);
        modifiers.OnAddModifiers += AddModifiers;
    }

    private void Update()
    {
        HandleCrouch();
        if (move != Vector2.zero)
        {
            Move(move);
        }
    }

    private void FixedUpdate()
    {
        rb.drag = defaultDrag * modifiers.GetValue(Modifier.DragModifier);
        if (moveForce != Vector3.negativeInfinity && moveForce != Vector3.positiveInfinity)
        {
            rb.AddForce(moveForce, ForceMode.Force);
        }
    }

    private void HandleCrouch()
    {
        if (crouching)
        {
            if (crouchTimer < 0)
            {
                crouchTimer = 0;
                previousCrouchSize = collision.localScale.y;
            }
            
            float lastFrameCrouchSize = collision.lossyScale.y;
            collision.localScale = new Vector3(collisionScale.x,
                Mathf.Lerp(previousCrouchSize, crouchSize, crouchTimer),
                collisionScale.z);
            transform.position -= Vector3.up * (lastFrameCrouchSize - collision.lossyScale.y);
            crouchTimer += Time.deltaTime * crouchSpeed / (previousCrouchSize - crouchSize);
        }
        else
        {
            if (crouchTimer > 0)
            {
                crouchTimer = 0;
                previousCrouchSize = collision.localScale.y;
            }

            float lastFrameCrouchSize = collision.lossyScale.y;
            collision.localScale = new Vector3(collisionScale.x,
                Mathf.Lerp(previousCrouchSize, collisionScale.y, -crouchTimer),
                collisionScale.z);
            transform.position -= Vector3.up * (lastFrameCrouchSize - collision.lossyScale.y);
            crouchTimer -= Time.deltaTime * crouchSpeed / (collisionScale.y - previousCrouchSize);
        }
    }

    public static MovementBehaviour GetMovementBehaviour(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<MovementBehaviour>(out MovementBehaviour movementBehaviour))
        {
            return movementBehaviour;
        }
        Debug.LogWarning("No MovementBehaviour found on " + gameObject.name + ".");
        return null;
    }

    private void AddModifiers()
    {
        if (!isGrounded)
        {
            modifiers.AddValue(Modifier.AccelerationModifier, airAccelerationModifier);
            modifiers.AddValue(Modifier.MaxSpeedModifier, airSpeedModifier);
            modifiers.AddValue(Modifier.DragModifier, airDrag);
        }

        if (isSprinting)
        {
            modifiers.AddValue(Modifier.MaxSpeedModifier, sprintMaxSpeedModifier);
            modifiers.AddValue(Modifier.AccelerationModifier, sprintAccelerationModifier);
        }

        if (crouching)
        {
            modifiers.AddValue(Modifier.AccelerationModifier, crouchAccelerationModifier);
            modifiers.AddValue(Modifier.MaxSpeedModifier, crouchSpeedModifier);
        }
    }

    public void Move(Vector2 direction)
    {
        float effectiveAcceleration = acceleration * modifiers.GetValue(Modifier.AccelerationModifier);
        float effectiveMaxSpeed = maxSpeed * modifiers.GetValue(Modifier.MaxSpeedModifier);

        Vector3 move = new Vector3(direction.x, 0, direction.y);
        if (moveInFacingDirection)
        {
            move = transform.TransformDirection(move);
        }
        moveForce = effectiveAcceleration * Mathf.Clamp(1 - new Vector2(rb.velocity.x, rb.velocity.z).magnitude / effectiveMaxSpeed, 0, 1) * move;
    }

    public void Crouch(bool isCrouching)
    {
        crouching = isCrouching;
    }

    public void ToggleCrouch()
    {
        crouching = !crouching;
    }

    public void Jump()
    {
        if (!isGrounded) return;

        modifiers.QueueModifierAction(() => modifiers.AddValue(Modifier.DragModifier, 0), 0.1f);

        float effectiveJumpForce = jumpForce * modifiers.GetValue(Modifier.JumpForceModifier);
        rb.velocity = new Vector3(rb.velocity.x, effectiveJumpForce, rb.velocity.z);
    }
}

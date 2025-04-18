// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

/// <summary>
/// A first-person character controller that uses physics-based movement.
/// </summary>
public class FirstPersonController : PlayerMovementBase
{
    [Header("Camera Control")]
    [Tooltip("Drag in the first-person camera.")]
    [SerializeField] private Camera firstPersonCamera;

    [Tooltip("Mouse input sensitivity of the first-person camera.")]
    [Range(0.1f, 8f), SerializeField] private float sensitivity = 2f;

    [Header("Ground Detection")]
    [Tooltip("Drag in a ground check transform positioned just under the player.")]
    [SerializeField] private Transform groundCheck;

    [Tooltip("The size of the circle that looks for the ground, centered at the ground check transform.")]
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Tooltip("Select the layer designated as the ground.")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Settings")]
    [Tooltip("The player's movement velocity.")]
    [SerializeField] private float movementSpeed = 5f;

    [Tooltip("A velocity multiplier to the movement speed when the sprint key is held.")]
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Jump Settings")]
    [Tooltip("Choose how many jumps the player is allowed before returning to the ground.")]
    [SerializeField] private int jumps = 1;

    [Tooltip("The strength of the player's jump.")]
    [SerializeField] private float jumpForce = 7f;

    [Tooltip("For how long before landing on the ground a jump input will be buffered.")]
    [SerializeField] private float jumpBuffer = 0.075f;

    [Tooltip("For how long after leaving the ground the player will still be able to jump.")]
    [SerializeField] private float coyoteTime = 0.125f;

    [Header("Gravity Settings")]
    [Tooltip("A fall speed multiplier to make jumping less floaty.")]
    [SerializeField] private float fallMultiplier = 2.5f;

    [Tooltip("For when the player is rising but the jump key is not held.")]
    [SerializeField] private float lowJumpFallMultiplier = 2f;

    [Tooltip("Cap for the maximum falling speed.")]
    [SerializeField] private float maxFallSpeed = 20f;

    private Rigidbody rb;
    private float pitch;
    private float yaw;
    private float sprintSpeed = 1f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private int jumpsRemaining;
    private bool jumpQueued;
    private bool isGrounded;
    private bool wasRunning;
    private bool canMove = true;
    private bool canLook = true;

    public void EnableMovement(bool movementEnabled)
    {
        canMove = movementEnabled;
    }

    public void EnableLooking(bool lookEnabled)
    {
        canLook = lookEnabled;
    }

    public void ResetMovement()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // If no camera is assigned, try to find main camera
        if (firstPersonCamera == null)
        {
            firstPersonCamera = Camera.main;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Handle jump input
        if (Input.GetButtonDown("Jump") && canMove)
        {
            if (jumpsRemaining > 0 || coyoteTimeCounter > 0f)
            {
                // Queue a jump so that during the next FixedUpdate frame the script knows that jump was pressed
                jumpQueued = true;
            }
            else
            {
                // If no jumps are available, start the jump buffer timer
                jumpBufferCounter = jumpBuffer;
            }
        }

        // Decrease jump buffer timer if active
        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (canLook)
        {
            Look();
        }
    }

    private void FixedUpdate()
    {
        // Keep track of whether the player was on the ground last FixedUpdate frame
        bool wasGrounded = isGrounded;

        // Check if the player is on the ground during this FixedUpdate frame
        isGrounded = CheckIfGrounded();

        if (isGrounded)
        {
            if (wasGrounded)
            {
                sprintSpeed = 1f;
            }
            else
            {
                OnLand?.Invoke(rb.linearVelocity.y);
            }

            // If the player is on the ground, restore their jumps and coyote time
            coyoteTimeCounter = coyoteTime;
            jumpsRemaining = jumps;

            // If jump buffer is active when landing, execute jump immediately
            if (jumpBufferCounter > 0f)
            {
                jumpQueued = true;
                jumpBufferCounter = 0f; // Reset buffer
            }
        }
        else
        {
            // Decrease coyote time if the player is in the air
            coyoteTimeCounter -= Time.fixedDeltaTime;

            // If the player moved off of the ground without jumping, still reduce their jumps by 1
            if (jumpsRemaining == jumps && coyoteTimeCounter <= 0f)
            {
                jumpsRemaining--;
            }

            // Apply custom gravity when in air
            ApplyCustomGravity();
        }

        Vector2 axis = Vector2.zero;

        if (canMove)
        {
            axis = new(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

            // Normalize axis to prevent faster diagonal movement
            if (axis.sqrMagnitude > 1f)
            {
                axis.Normalize();
            }

            axis *= movementSpeed;

            if (isGrounded && Input.GetButton("Fire3"))
            {
                sprintSpeed = sprintMultiplier;
            }

            axis *= sprintSpeed;

            Vector3 forward = new(-firstPersonCamera.transform.right.z, 0f, firstPersonCamera.transform.right.x);

            // Preserve gravity-affected Y velocity
            float currentYVelocity = rb.linearVelocity.y;
            Vector3 horizontalVelocity = forward * axis.x + firstPersonCamera.transform.right * axis.y;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, currentYVelocity, horizontalVelocity.z);
        }

        // Keep track of whether the player is running this FixedUpdate frame
        bool isRunning = axis != Vector2.zero && isGrounded;
        if (isRunning != wasRunning)
        {
            OnRunningStateChanged?.Invoke(isRunning);
            wasRunning = isRunning;
        }

        // Handle jumping
        if (jumpQueued)
        {
            // Check if this was a jump off of the ground, or a mid-air jump
            if (isGrounded || coyoteTimeCounter > 0f)
            {
                OnJump?.Invoke();
            }
            else
            {
                OnMidAirJump?.Invoke();
            }

            // Directly set the player's vertical velocity to the jump force
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);

            // Disable coyote time once the jump is used
            coyoteTimeCounter = 0f;

            // Reduce jumps remaining and reset jumpQueued
            jumpsRemaining--;
            jumpQueued = false;
        }
    }

    private void ApplyCustomGravity()
    {
        // Apply stronger gravity when falling
        float gravityMultiplier;

        if (rb.linearVelocity.y < 0)
        {
            // Falling down - apply fall multiplier
            gravityMultiplier = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Rising but jump button not held - apply low jump multiplier for quick arc
            gravityMultiplier = lowJumpFallMultiplier;
        }
        else
        {
            // Normal gravity when rising and holding jump
            gravityMultiplier = 1f;
        }

        // Apply the extra gravity
        Vector3 extraGravity = (Physics.gravity * gravityMultiplier) - Physics.gravity;
        rb.AddForce(extraGravity, ForceMode.Acceleration);

        // Cap maximum fall speed
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, rb.linearVelocity.z);
        }
    }

    private void Look()
    {
        pitch -= Input.GetAxisRaw("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        yaw += Input.GetAxisRaw("Mouse X") * sensitivity;
        firstPersonCamera.transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
    }

    private bool CheckIfGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }
}
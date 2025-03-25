using UnityEngine;

/// <summary>
/// A basic player controller with simple running and jumping behavior.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Drag in the player's animator to play animations for running and jumping.")]
    [SerializeField] protected Animator animator;

    [Header("Ground Detection")]
    [Tooltip("Drag in a transform positioned at the player's feet. The ground check transform should be a child of the player.")]
    [SerializeField] protected Transform groundCheck;

    [Tooltip("The size of the circle that looks for the ground, centered at the ground check transform.")]
    [SerializeField] protected float groundCheckRadius = 0.1f;

    [Tooltip("Select the layer designated as the ground.")]
    [SerializeField] protected LayerMask groundLayer;

    [Header("Movement Settings")]
    [Tooltip("The player's movement velocity.")]
    [SerializeField] protected float movementSpeed = 15f;

    [Tooltip("Choose how many jumps the player is allowed before returning to the ground.")]
    [SerializeField] protected int jumps = 1;

    [Tooltip("The strength of the player's jump.")]
    [SerializeField] protected float jumpForce = 20f;

    [Tooltip("For how long before landing on the ground a jump input will be buffered.")]
    [SerializeField] protected float jumpBuffer = 0.075f;

    [Tooltip("For how long after leaving the ground the player will still be able to jump.")]
    [SerializeField] protected float coyoteTime = 0.125f;

    // Events for different player actions
    // These are C# events (not UnityEvents) and must be accessed via code
    public event System.Action OnJump;
    public event System.Action OnMidAirJump;
    public event System.Action OnLand;
    public event System.Action<bool> OnRunningStateChanged;
    public event System.Action OnDash; // Used by PlayerMovementAdvanced

    protected Rigidbody2D rb;
    protected float coyoteTimeCounter;
    protected float jumpBufferCounter;
    protected float landingThreshold = 0.15f; // How long the player needs to fall to count as landing
    protected int jumpsRemaining;
    protected bool canMove = true;
    protected bool isFacingRight = true;
    protected bool isGrounded;
    protected bool jumpQueued;
    protected bool wasRunning;

    public virtual void EnableMovement(bool isEnabled)
    {
        canMove = isEnabled;
    }

    public virtual void ResetMovement()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public virtual void AdjustJumps(int adjustment)
    {
        jumps += adjustment;
    }

    public virtual void SetJumps(int newJumps)
    {
        jumps = newJumps;
    }

    protected virtual void OnJumpEvent()
    {
        OnJump?.Invoke();
    }

    protected virtual void OnMidAirJumpEvent()
    {
        OnMidAirJump?.Invoke();
    }

    protected virtual void OnLandEvent()
    {
        OnLand?.Invoke();
    }

    protected virtual void OnRunningStateChangedEvent(bool isRunning)
    {
        OnRunningStateChanged?.Invoke(isRunning);
    }

    protected virtual void OnDashEvent()
    {
        OnDash?.Invoke();
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsRemaining = jumps;
    }

    protected virtual void Update()
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
    }

    protected virtual void FixedUpdate()
    {
        // Keep track of whether the player was on the ground last FixedUpdate frame
        bool wasGrounded = isGrounded;

        // Check if the player is on the ground during this FixedUpdate frame
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            // Check if the player landed this FixedUpdate frame
            if (!wasGrounded && coyoteTimeCounter < -landingThreshold)
            {
                OnLand?.Invoke();
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
            if (wasGrounded && jumpsRemaining == jumps)
            {
                jumpsRemaining--;
            }
        }

        float moveInput = 0f;

        if (canMove)
        {
            // Get the horizontal axis (A and D keys or left and right arrow keys by default)
            moveInput = Input.GetAxisRaw("Horizontal");

            // Set the player's horizontal velocity to the input multiplied by the movement speed variable
            rb.linearVelocity = new Vector2(moveInput * movementSpeed, rb.linearVelocityY);

            if (moveInput != 0)
            {
                // Check if the player's facing direction should be flipped
                CheckDirectionToFace(moveInput > 0);
            }
        }

        // Keep track of whether the player is running this FixedUpdate frame
        bool isRunning = moveInput != 0 && isGrounded;
        if (isRunning != wasRunning)
        {
            OnRunningStateChanged?.Invoke(isRunning);
            wasRunning = isRunning;
        }

        // If assigned, set the animator's "IsRunning" parameter true or false based on whether horizontal movement is being inputted
        if (animator != null)
        {
            animator.SetBool("IsRunning", isRunning);
        }

        // Handle jumping
        if (jumpQueued)
        {
            // Directly set the player's vertical velocity to the jump force
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);

            // Disable coyote time once the jump is used
            coyoteTimeCounter = 0f;

            // Reduce jumps remaining and reset jumpQueued
            jumpsRemaining--;
            jumpQueued = false;

            // If assigned, give the animator a "Jump" trigger
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
 
            // Check if this was a jump off of the ground, or a mid-air jump
            if (isGrounded || coyoteTimeCounter > 0f)
            {
                OnJump?.Invoke();
            }
            else
            {
                OnMidAirJump?.Invoke();
            }
        }
    }

    public virtual void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
        {
            Turn();
        }
    }

    protected virtual void Turn()
    {
        // Flip the player's transform on the x-axis
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        isFacingRight = !isFacingRight;
    }
}

// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// A basic platformer player controller with simple running and jumping behavior.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement2D : PlayerMovementBase
    {
        [System.Serializable]
        public class AnimationParameters
        {
            [Tooltip("Bool parameter.")]
            public string isRunningBool = "IsRunning";

            [Tooltip("Trigger parameter.")]
            public string jumpTrigger = "Jump";

            [Tooltip("(Only used by PlayerMovementAdvanced)\nBool parameter.")]
            public string isDashingBool = "IsDashing";
        }

        [Header("Animation Settings")]
        [Tooltip("Drag in the player's animator to play animations for running and jumping.")]
        [SerializeField] protected Animator animator;
        [SerializeField] protected AnimationParameters animationParameters;

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

        [Tooltip("The button input used for jump. Set to the space bar by default.")]
        [SerializeField] protected KeyCode jumpKey = KeyCode.Space;

        [Tooltip("Choose how many jumps the player is allowed before returning to the ground.")]
        [SerializeField] protected int jumps = 1;

        [Tooltip("The strength of the player's jump.")]
        [SerializeField] protected float jumpForce = 20f;

        [Tooltip("For how long before landing on the ground a jump input will be buffered.")]
        [SerializeField] protected float jumpBuffer = 0.075f;

        [Tooltip("For how long after leaving the ground the player will still be able to jump.")]
        [SerializeField] protected float coyoteTime = 0.125f;

        [Tooltip("Optional: Drag in a transform that will point in the direction the player is facing.")]
        [SerializeField] private Transform facingTransform;

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

            if (!isEnabled)
            {
                ResetMovement();
            }
        }

        public virtual void ResetMovement()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        public virtual void EnableAndResetMovement(bool isEnabled)
        {
            canMove = isEnabled;
            ResetMovement();
        }

        public virtual void AdjustJumps(int adjustment)
        {
            jumps += adjustment;
        }

        public virtual void SetJumps(int newJumps)
        {
            jumps = newJumps;
        }

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            jumpsRemaining = jumps;
        }

        protected virtual void Update()
        {
            // Handle jump input
            if (Input.GetKeyDown(jumpKey) && canMove)
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
                    OnLandEvent();
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
                OnRunningStateChangedEvent(isRunning);
                wasRunning = isRunning;
            }

            // If assigned, set the animator's "IsRunning" parameter true or false based on whether there is horizontal movement input
            if (animator != null)
            {
                animator.SetBool(animationParameters.isRunningBool, isRunning);
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
                    animator.SetTrigger(animationParameters.jumpTrigger);
                }

                // Check if this was a jump off of the ground, or a mid-air jump
                if (isGrounded || coyoteTimeCounter > 0f)
                {
                    OnJumpEvent();
                }
                else
                {
                    OnMidAirJumpEvent();
                }
            }
        }

        protected void CheckDirectionToFace(bool isMovingRight)
        {
            if (isMovingRight != isFacingRight)
            {
                Turn();
            }
        }

        public void TurnRight()
        {
            if (!isFacingRight)
            {
                Turn();
            }
        }

        public void TurnLeft()
        {
            if (isFacingRight)
            {
                Turn();
            }
        }

        public void Turn()
        {
            // Flip the player's transform on the x-axis
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            isFacingRight = !isFacingRight;

            if (facingTransform != null)
            {
                facingTransform.rotation *= Quaternion.Euler(0f, 180f, 0f);
            }
        }

        protected virtual void OnValidate()
        {
            // Enforce value ranges in the inspector
            groundCheckRadius = Mathf.Max(0.0001f, groundCheckRadius);
            movementSpeed = Mathf.Max(0f, movementSpeed);
            jumps = Mathf.Max(0, jumps);
            jumpForce = Mathf.Max(0f, jumpForce);
            jumpBuffer = Mathf.Clamp(jumpBuffer, 0f, 1f);
            coyoteTime = Mathf.Clamp(coyoteTime, 0f, 1f);
        }
    }
}
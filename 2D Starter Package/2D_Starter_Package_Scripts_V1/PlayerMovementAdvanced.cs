// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// A more sophisticated player controller that extends PlayerMovement2D to add dashing and wall jumping functionality.
    /// </summary>
    public class PlayerMovementAdvanced : PlayerMovement2D
    {
        public enum DashCondition : byte
        {
            None,
            Anytime,
            WhenGrounded,
            WhenAirborne
        }

        public enum DashDirection : byte
        {
            Horizontal,
            Vertical,
            UpwardsOnly,
            HorizontalAndVertical,
            OmniDirectional
        }

        [Header("Dash Settings")]
        [Tooltip("The force applied during a dash.")]
        [SerializeField] private float dashForce = 30f;

        [Tooltip("The duration of the dash in seconds.")]
        [SerializeField] private float dashDuration = 0.15f;

        [Tooltip("The cooldown time before the player can dash again.")]
        [SerializeField] private float dashCooldown = 0.5f;

        [Tooltip("Select the conditions that allow for a dash.")]
        [SerializeField] private DashCondition dashCondition = DashCondition.Anytime;

        [Tooltip("Select the dash direction mode.")]
        [SerializeField] private DashDirection dashDirectionMode = DashDirection.Horizontal;

        [Tooltip("If checked, the player's gravity will be temporarily disabled during the dash.")]
        [SerializeField] private bool disableGravityDuringDash = true;

        [Header("Wall Jumping")]
        [Tooltip("Enable/disable wall jumping and sliding.")]
        [SerializeField] private bool canWallJump = true;

        [Tooltip("Layers that are considered walls for wall jumping and sliding.")]
        [SerializeField] private LayerMask wallLayer;

        [Tooltip("Transform used to check for wall contact (should be placed near the player's side).")]
        [SerializeField] private Transform wallCheck;

        [Tooltip("Radius for wall detection.")]
        [SerializeField] private float wallCheckRadius = 0.2f;

        [Tooltip("Sliding speed when pressing against a wall.")]
        [SerializeField] private float wallSlideSpeed = 3f;

        [Tooltip("Force applied when jumping off a wall.")]
        [SerializeField] private Vector2 wallJumpForce = new(12f, 20f);

        [Tooltip("How long the player is unable to move after a wall jump.")]
        [SerializeField] private float wallJumpLockTime = 0.2f;

        [Tooltip("If true, wall jumping will require remaining jumps and will consume jumps.")]
        [SerializeField] private bool wallJumpingDepletesJumps = false;

        private Vector2 dashDirection;
        private float dashCooldownCounter;
        private float gravityScale;
        private bool isDashing;
        private bool dashQueued;

        private Vector2 wallNormal;
        private float wallJumpLockCounter;
        private bool isTouchingWall;
        private bool isWallSliding;
        private bool isWallJumping;

        public void SetDash(int newCondition)
        {
            dashCondition = (DashCondition)newCondition;
        }

        public void SetWallJumping(bool enabled)
        {
            canWallJump = enabled;
        }

        protected override void Start()
        {
            base.Start();
            gravityScale = rb.gravityScale;
        }

        protected override void Update()
        {
            base.Update();

            // Handle dash input (using left shift key by default)
            if (Input.GetKeyDown(KeyCode.LeftShift) && canMove && dashCooldownCounter <= 0f && !isDashing)
            {
                // Check if dash is allowed based on ground state
                if (dashCondition == DashCondition.Anytime ||
                   (dashCondition == DashCondition.WhenGrounded && isGrounded) ||
                   (dashCondition == DashCondition.WhenAirborne && !isGrounded))
                {
                    // Store the dash direction based on current input
                    dashDirection = GetDashDirection();

                    // Only queue the dash if we have a valid direction
                    if (dashDirection.magnitude > 0.1f)
                    {
                        dashQueued = true;
                    }
                }
            }

            // Update dash cooldown
            if (dashCooldownCounter > 0f)
            {
                dashCooldownCounter -= Time.deltaTime;
            }

            // Update animator if we're dashing
            if (animator != null)
            {
                animator.SetBool(animationParameters.IsDashing, isDashing);
            }

            // Handle wall jump input
            if (Input.GetButtonDown("Jump") && isWallSliding && canMove && canWallJump)
            {
                if (!wallJumpingDepletesJumps || jumpsRemaining > 0)
                {
                    PerformWallJump();

                    // Cancel any pending jump
                    jumpQueued = false;
                    jumpBufferCounter = 0f;
                }
            }
        }

        private Vector2 GetDashDirection()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector2 direction = Vector2.zero;

            // Determine dash direction based on the selected mode
            switch (dashDirectionMode)
            {
                case DashDirection.Horizontal:
                    direction = new Vector2(horizontal, 0f);
                    break;
                case DashDirection.Vertical:
                    direction = new Vector2(0f, vertical);
                    break;
                case DashDirection.UpwardsOnly:
                    direction = Vector2.up;
                    break;
                case DashDirection.HorizontalAndVertical:
                    if (horizontal != 0f)
                    {
                        direction.x = horizontal;
                    }
                    if (vertical != 0f)
                    {
                        direction.y = vertical;
                    }
                    break;
                case DashDirection.OmniDirectional:
                    direction = new Vector2(horizontal, vertical);
                    break;
                default:
                    break;
            }

            // If no direction input, use the facing direction
            if (direction.magnitude < 0.1f)
            {
                direction = new Vector2(isFacingRight ? 1f : -1f, 0f);
            }

            // Normalize the direction
            if (direction.magnitude > 0.1f)
            {
                direction.Normalize();
            }

            return direction;
        }

        protected override void FixedUpdate()
        {
            // If dashing, skip the normal movement handling from the base class
            if (isDashing)
            {
                // Just handle ground detection from the base class
                bool wasGrounded = isGrounded;
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

                if (isGrounded && !wasGrounded && coyoteTimeCounter < -0.15f)
                {
                    OnLandEvent();
                }

                return;
            }

            // If wall jumping, skip the normal movement handling from the base class 
            if (!isWallJumping || !canWallJump)
            {
                base.FixedUpdate();
            }
            else
            {
                // Ground detection
                bool wasGrounded = isGrounded;
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

                if (isGrounded && !wasGrounded && coyoteTimeCounter < -landingThreshold)
                {
                    OnLandEvent();
                }
            }

            // Handle dash if queued and not currently dashing
            if (dashQueued && !isDashing)
            {
                StartCoroutine(PerformDash());
                dashQueued = false;
            }

            // Wall detection
            isTouchingWall = false;
            Collider2D wallCollider = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
            if (wallCollider != null)
            {
                isTouchingWall = true;
                wallNormal = wallCollider.transform.position.x > transform.position.x ? Vector2.left : Vector2.right;
            }

            // Handle wall sliding
            if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
            {
                // Get the current horizontal input
                float horizontalInput = Input.GetAxisRaw("Horizontal");

                // Check if player is pressing toward the wall
                bool pressingTowardWall = (wallNormal.x < 0 && horizontalInput > 0) || (wallNormal.x > 0 && horizontalInput < 0);

                if (pressingTowardWall)
                {
                    isWallSliding = true;
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
                }
                else
                {
                    isWallSliding = false;
                    // Let normal gravity apply
                }
            }
            else
            {
                isWallSliding = false;
            }

            // Handle wall jump lock
            if (wallJumpLockCounter > 0)
            {
                wallJumpLockCounter -= Time.fixedDeltaTime;
            }
        }

        private void PerformWallJump()
        {
            isWallJumping = true;
            wallJumpLockCounter = wallJumpLockTime;

            // Apply force in the opposite direction of the wall
            Vector2 jumpDirection = new(wallNormal.x * wallJumpForce.x, wallJumpForce.y);
            rb.linearVelocity = jumpDirection;

            // Flip player to face away from the wall
            if (wallNormal.x > 0 && !isFacingRight || wallNormal.x < 0 && isFacingRight)
            {
                Turn();
            }

            // Reset coyote time and jump buffer
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;

            if (wallJumpingDepletesJumps)
            {
                jumpsRemaining--;
            }

            OnMidAirJumpEvent();

            Invoke(nameof(ResetWallJump), wallJumpLockTime);
        }

        private void ResetWallJump()
        {
            isWallJumping = false;
        }

        private IEnumerator PerformDash()
        {
            // Set dashing state
            isDashing = true;

            // Trigger the dash event
            OnDashEvent();

            // Temporarily disable gravity if needed
            if (disableGravityDuringDash)
            {
                rb.gravityScale = 0f;
            }

            // Apply dash force in the dash direction
            rb.linearVelocity = dashDirection * dashForce;

            // If dashing diagonally, make sure player faces the correct direction
            if (dashDirection.x > 0 && !isFacingRight)
            {
                Turn();
            }
            else if (dashDirection.x < 0 && isFacingRight)
            {
                Turn();
            }

            // Wait for the dash duration
            yield return new WaitForSeconds(dashDuration);

            // Reset dashing state
            isDashing = false;

            // Restore gravity
            if (disableGravityDuringDash)
            {
                rb.gravityScale = gravityScale;
            }

            // Start the cooldown
            dashCooldownCounter = dashCooldown;
        }
    }
}
// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// A third-person character controller that uses physics-based movement.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ThirdPersonController : PlayerMovementBase
    {
        [System.Serializable]
        public class AnimationParameters
        {
            [Tooltip("Bool parameter.")]
            public string isRunningBool = "IsRunning";

            [Tooltip("Trigger parameter.")]
            public string jumpTrigger = "Jump";
        }

        [Header("Camera Settings")]
        [Tooltip("If true, mouse input will not move the camera.")]
        [SerializeField] private bool lockCamera = false;

        [Tooltip("Mouse input sensitivity of the third-person camera.")]
        [Range(0.1f, 8f), SerializeField] private float sensitivity = 2f;

        [Tooltip("The point that the camera will pivot around. Use a child transform on the player to offset it.")]
        [SerializeField] private Transform cameraPivot;

        [Tooltip("Assign the main camera.")]
        [SerializeField] private Transform cameraTransform;

        [Tooltip("The distance between cameraPivot and the camera.")]
        [SerializeField] private float cameraDistance = 5f;

        [Tooltip("How close the camera can get to the cameraPivot.")]
        [SerializeField] private float minDistance = 1f;

        [Tooltip("How far the camera can get to the cameraPivot.")]
        [SerializeField] private float maxDistance = 5f;

        [Tooltip("How fast the camera will move in or out when colliding with obstacles.")]
        [SerializeField] private float cameraDistanceSpeed = 15f;

        [Header("Animation Settings")]
        [Tooltip("Optional: Assign the player's animator to add movement and jumping animation parameters.")]
        [SerializeField] private Animator animator;
        [SerializeField] protected AnimationParameters animationParameters;

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

        [Tooltip("How quickly the player rotates to the input direction.")]
        [SerializeField] private float rotationSpeed = 15f;

        [Tooltip("The button input used for sprinting. Set to left shift by default.")]
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

        [Tooltip("A velocity multiplier to the movement speed when the sprint key is held.")]
        [SerializeField] private float sprintMultiplier = 1.5f;

        [Header("Jump Settings")]
        [Tooltip("The button input used for jump. Set to the space bar by default.")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;

        [Tooltip("Choose how many jumps the player is allowed before returning to the ground.")]
        [SerializeField] private int jumps = 1;

        [Tooltip("The strength of the player's jump.")]
        [SerializeField] private float jumpForce = 5f;

        [Tooltip("For how long before landing on the ground a jump input will be buffered.")]
        [SerializeField] private float jumpBuffer = 0.075f;

        [Tooltip("For how long after leaving the ground the player will still be able to jump.")]
        [SerializeField] private float coyoteTime = 0.125f;

        private Rigidbody rb;
        private Vector3 moveInput;
        private float pitch = 0f;
        private float yaw = 0f;
        private float sprintSpeed = 1f;
        private float currentCameraDistance;
        private float coyoteTimeCounter;
        private float jumpBufferCounter;
        private int jumpsRemaining;
        private bool jumpQueued;
        private bool isGrounded;
        private bool wasRunning;
        private bool canMove = true;

        public void EnableMovement(bool movementEnabled)
        {
            canMove = movementEnabled;
        }

        public void LockCamera(bool cameraLocked)
        {
            lockCamera = cameraLocked;
        }

        public void ResetMovement()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        public void EnableAndResetMovement(bool isEnabled)
        {
            canMove = isEnabled;
            ResetMovement();
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            yaw = transform.eulerAngles.y;
            currentCameraDistance = cameraDistance;

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            Movement();
        }

        private void LateUpdate()
        {
            HandleCamera();
        }

        private void HandleInput()
        {
            if (!canMove)
            {
                return;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            moveInput = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
            moveInput.y = 0;

            if (moveInput.sqrMagnitude > 0.01f) // Prevents rotating when input is very low
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveInput);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));

                if (animator != null)
                {
                    animator.SetBool(animationParameters.isRunningBool, true);
                }
            }
            else
            {
                rb.angularVelocity = Vector3.zero;

                if (animator != null)
                {
                    animator.SetBool(animationParameters.isRunningBool, false);
                }
            }

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

        private void Movement()
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
                    OnLandEvent(rb.linearVelocity.y);
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
            }

            if (canMove)
            {
                if (isGrounded && Input.GetKey(sprintKey))
                {
                    sprintSpeed = sprintMultiplier;
                }

                // Normalize moveInput to prevent faster diagonal movement
                Vector3 targetVelocity = movementSpeed * sprintSpeed * moveInput.normalized;
                targetVelocity.y = rb.linearVelocity.y;
                rb.linearVelocity = targetVelocity;
            }

            // Keep track of whether the player is running this FixedUpdate frame
            bool isRunning = moveInput != Vector3.zero && isGrounded;
            if (isRunning != wasRunning)
            {
                OnRunningStateChangedEvent(isRunning);
                wasRunning = isRunning;
            }

            if (jumpQueued)
            {
                if (isGrounded || coyoteTimeCounter > 0f)
                {
                    OnJumpEvent();
                }
                else
                {
                    OnMidAirJumpEvent();
                }

                // Directly set the player's vertical velocity to the jump force
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);

                // Disable coyote time once the jump is used
                coyoteTimeCounter = 0f;

                // Reduce jumps remaining and reset jumpQueued
                jumpsRemaining--;
                jumpQueued = false;

                if (animator != null)
                {
                    animator.SetTrigger(animationParameters.jumpTrigger);
                }
            }
        }

        private void HandleCamera()
        {
            if (!lockCamera)
            {
                yaw += Input.GetAxis("Mouse X") * sensitivity;
                pitch -= Input.GetAxis("Mouse Y") * sensitivity;
                pitch = Mathf.Clamp(pitch, -30f, 60f);
            }

            Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0);
            Vector3 desiredPosition = cameraPivot.position - (cameraRotation * Vector3.forward * cameraDistance);

            if (Physics.Raycast(cameraPivot.position, (desiredPosition - cameraPivot.position).normalized, out RaycastHit hit, maxDistance))
            {
                float targetDistance = Mathf.Clamp(hit.distance - 0.5f, minDistance, maxDistance);
                currentCameraDistance = Mathf.Lerp(currentCameraDistance, targetDistance, Time.deltaTime * cameraDistanceSpeed);
            }
            else
            {
                currentCameraDistance = Mathf.Lerp(currentCameraDistance, maxDistance, Time.deltaTime * cameraDistanceSpeed);
            }

            cameraTransform.position = cameraPivot.position - (cameraRotation * Vector3.forward * currentCameraDistance);
            cameraTransform.LookAt(cameraPivot);
        }

        private bool CheckIfGrounded()
        {
            return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        }

        private void OnValidate()
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
// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// A basic top-down player controller with simple physics-based movement.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovementTopDown : PlayerMovementBase
    {
        [System.Serializable]
        public class AnimationParameters
        {
            [Tooltip("Bool parameter.")]
            public string isRunningBool = "IsRunning";

            [Tooltip("Float parameter.")]
            public string inputXFloat = "InputX";

            [Tooltip("Float parameter.")]
            public string inputYFloat = "InputY";

            [Tooltip("Float parameter.")]
            public string lastInputXFloat = "LastInputX";

            [Tooltip("Float parameter.")]
            public string lastInputYFloat = "LastInputY";
        }

        public enum MovementMode : byte
        {
            EightDirections,
            FourDirections,
            VerticalOnly,
            HorizontalOnly
        }

        [Header("Animation Settings")]
        [Tooltip("Drag in the player's animator to play running and idle animations.")]
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationParameters animationParameters;

        [Header("Movement Settings")]
        [Tooltip("The player's movement velocity.")]
        [SerializeField] private float movementSpeed = 8f;

        [Tooltip("Choose which directions the player can move in.")]
        [SerializeField] private MovementMode movementMode = MovementMode.EightDirections;

        [Header("Facing Settings")]
        [Tooltip("Optional: Drag in a transform that will point in the direction the player is facing.")]
        [SerializeField] private Transform facingTransform;

        [Tooltip("How far the facingTransform will extend from the player.")]
        [SerializeField] private float facingTransformDistance = 1f;

        private Rigidbody2D rb;
        private Vector2 movementInput;
        private bool canMove = true;

        public void EnableMovement(bool isEnabled)
        {
            canMove = isEnabled;
        }

        public void ResetMovement()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        public void EnableAndResetMovement(bool isEnabled)
        {
            canMove = isEnabled;
            ResetMovement();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!canMove)
            {
                movementInput = Vector2.zero;
                return;
            }

            // Get input using the Input Manager (old) system
            // Set to WASD or the arrow keys by default
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            switch (movementMode)
            {
                case MovementMode.EightDirections:
                    movementInput = new Vector2(inputX, inputY);
                    break;

                case MovementMode.FourDirections:
                    if (Mathf.Abs(inputX) > Mathf.Abs(inputY))
                    {
                        movementInput = new Vector2(Mathf.Sign(inputX), 0);
                    }
                    else if (Mathf.Abs(inputY) > 0)
                    {
                        movementInput = new Vector2(0, Mathf.Sign(inputY));
                    }
                    else
                    {
                        movementInput = Vector2.zero;
                    }
                    break;

                case MovementMode.HorizontalOnly:
                    movementInput = new Vector2(inputX, 0);
                    break;

                case MovementMode.VerticalOnly:
                    movementInput = new Vector2(0, inputY);
                    break;
            }

            UpdateFacingDirection();
        }

        private void FixedUpdate()
        {
            // Normalize the vector to prevent faster diagonal movement
            Vector2 normalizedInput = movementInput;
            if (movementInput.magnitude > 1f)
            {
                normalizedInput = movementInput.normalized;
            }

            // Move the player's rigidbody
            rb.linearVelocity = normalizedInput * movementSpeed;

            bool isRunning = movementInput != Vector2.zero;
            OnRunningStateChangedEvent(isRunning);

            // Update the animation parameters
            if (animator != null)
            {
                animator.SetBool(animationParameters.isRunningBool, isRunning);
                animator.SetFloat(animationParameters.inputXFloat, movementInput.x);
                animator.SetFloat(animationParameters.inputYFloat, movementInput.y);

                if (isRunning)
                {
                    animator.SetFloat(animationParameters.lastInputXFloat, movementInput.x);
                    animator.SetFloat(animationParameters.lastInputYFloat, movementInput.y);
                }
            }
        }

        private void UpdateFacingDirection()
        {
            if (movementInput == Vector2.zero || facingTransform == null)
            {
                return;
            }

            Vector3 direction;

            switch (movementMode)
            {
                case MovementMode.FourDirections:
                    if (Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.y))
                    {
                        direction = movementInput.x > 0 ? Vector3.right : Vector3.left;
                    }
                    else
                    {
                        direction = movementInput.y > 0 ? Vector3.up : Vector3.down;
                    }
                    break;

                default:
                    direction = new Vector3(movementInput.x, movementInput.y, 0).normalized;
                    break;
            }

            // Position the facingTransform at an offset from the player
            facingTransform.localPosition = direction * facingTransformDistance;

            // Rotate the facingTransform to look in the movement direction (X-axis facing out)
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                facingTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }
}
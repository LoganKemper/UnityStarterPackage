// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// A basic top-down player controller with simple movement.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovementTopDown : PlayerMovementBase
    {
        [Tooltip("Drag in the player's animator to play animations for running.")]
        [SerializeField] private Animator animator;

        [Header("Movement Settings")]
        [Tooltip("The player's movement velocity.")]
        [SerializeField] private float movementSpeed = 8f;

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
            if (canMove)
            {
                // Get input using the old Input system
                // Set to WASD or the arrow keys by default
                movementInput.x = Input.GetAxisRaw("Horizontal");
                movementInput.y = Input.GetAxisRaw("Vertical");

                // Normalize the vector to prevent faster diagonal movement
                if (movementInput.magnitude > 1)
                {
                    movementInput = movementInput.normalized;
                }
            }
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = movementInput * movementSpeed;

            bool isRunning = movementInput != Vector2.zero;
            OnRunningStateChangedEvent(isRunning);

            if (animator != null)
            {
                animator.SetBool("IsRunning", isRunning);
                animator.SetFloat("InputX", movementInput.x);
                animator.SetFloat("InputY", movementInput.y);
            }
        }
    }
}
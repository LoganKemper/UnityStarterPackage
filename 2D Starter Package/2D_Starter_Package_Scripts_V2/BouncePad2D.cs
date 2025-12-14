// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// A bounce pad that applies an instant force to any GameObject with a Rigidbody2D that collides with it.
    /// </summary>
    public class BouncePad2D : MonoBehaviour
    {
        // Defines possible bounce directions
        public enum BounceDirection : byte
        {
            Up,
            Down,
            Left,
            Right,
            UpRight,
            UpLeft,
            DownRight,
            DownLeft
        }

        public enum ForceMode : byte
        {
            AddForce,
            OverrideForce
        }

        [Tooltip("Enter the tag name that should register collisions. Leave blank for any object to be affected.")]
        [SerializeField] private string tagName;

        [Tooltip("The amount of force applied to the colliding GameObject's Rigidbody2D.")]
        [SerializeField] private float bounceForce = 25f;

        [Tooltip("Choose the direction of the bounce force.")]
        [SerializeField] private BounceDirection bounceDirection = BounceDirection.Up;

        [Tooltip("Choose Add Force to apply the force in addition to the Rigidbody2D's current force. Choose Override Force to replace the current force with the new bounce force.")]
        [SerializeField] private ForceMode forceMode = ForceMode.AddForce;

        [Space(20)]
        [SerializeField] private UnityEvent onBounce;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Check if the colliding object has the required tag (if specified)
            if (string.IsNullOrEmpty(tagName) || collision.collider.CompareTag(tagName))
            {
                // Try to access the rigidbody on the colliding GameObject
                if (collision.gameObject.TryGetComponent(out Rigidbody2D rb))
                {
                    // Determine the bounce force direction
                    Vector2 forceDirection = GetDirectionVector(bounceDirection);

                    if (forceMode == ForceMode.AddForce)
                    {
                        // Add force to the rigidbody
                        rb.AddForce(forceDirection * bounceForce, ForceMode2D.Impulse);
                    }
                    else if (forceMode == ForceMode.OverrideForce)
                    {
                        // Replace the velocity with a new one in the force direction
                        rb.linearVelocity = forceDirection * bounceForce;
                    }

                    onBounce.Invoke();
                }
            }
        }

        // Convert the bounce direction into a unit vector
        private Vector2 GetDirectionVector(BounceDirection direction)
        {
            return direction switch
            {
                BounceDirection.Up => Vector2.up,
                BounceDirection.Down => Vector2.down,
                BounceDirection.Left => Vector2.left,
                BounceDirection.Right => Vector2.right,
                BounceDirection.UpRight => new Vector2(1, 1).normalized,
                BounceDirection.UpLeft => new Vector2(-1, 1).normalized,
                BounceDirection.DownRight => new Vector2(1, -1).normalized,
                BounceDirection.DownLeft => new Vector2(-1, -1).normalized,
                _ => Vector2.up
            };
        }
    }
}
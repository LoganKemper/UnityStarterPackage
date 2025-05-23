// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Attach to a projectile prefab to give it launching and destroying behavior.
    /// </summary>
    public class Projectile2D : MonoBehaviour
    {
        [Tooltip("Drag in the projectile's Rigidbody2D.")]
        [SerializeField] private Rigidbody2D m_Rigidbody2D;

        [Tooltip("How long in seconds until the projectile disappears.")]
        [SerializeField] private float lifetime = 2f;

        [Tooltip("Speed of the projectile relative to the speed of GameObject it came from.")]
        [SerializeField] private float relativeVelocity = 1f;

        [Tooltip("Whether the projectile should destroy itself when its collider is set to trigger and it collides with another collider.")]
        [SerializeField] private bool destroyOnTrigger = false;

        [Tooltip("If true, the projectile won't destroy itself if it collides with the GameObject it came from.")]
        [SerializeField] private bool dontCollideWithOrigin = true;

        [Tooltip("If true, the projectile will point towards the direction it's moving in (assuming the sprite is facing right).")]
        [SerializeField] private bool rotateToTarget = false;

        private GameObject launcherOrigin;

        private void Start()
        {
            // Destroy the projectile after its lifetime expires
            Destroy(gameObject, lifetime);

            // If the Rigidbody2D hasn't been assigned, try to find it on this GameObject
            if (m_Rigidbody2D == null)
            {
                m_Rigidbody2D = GetComponent<Rigidbody2D>();
            }
        }

        // Called from the script that created this projectile to launch it
        // This Launch method takes in facingRight to determine which direction to travel in
        public void Launch(float emitVelocity, bool facingRight, GameObject origin)
        {
            // Cache the GameObject that this projectile originated from
            if (origin != null)
            {
                launcherOrigin = origin;
            }

            m_Rigidbody2D.linearVelocityX = emitVelocity * relativeVelocity;

            // Flip the transform on the x-axis if it's not facing right
            if (!facingRight)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        // Called from the script that created this projectile to launch it
        // This Launch method takes in a direction vector to travel in any direction
        public void Launch(Vector2 direction, float emitVelocity, GameObject origin)
        {
            // Cache the GameObject that this projectile originated from
            if (origin != null)
            {
                launcherOrigin = origin;
            }

            m_Rigidbody2D.linearVelocity = direction * emitVelocity;

            if (rotateToTarget)
            {
                // Calculate the rotation angle in degrees
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Apply rotation (assuming projectile's default forward direction is to the right)
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
            else
            {
                // Flip the transform on the x-axis if the direction in pointing left
                if (direction.x < 0)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!destroyOnTrigger)
            {
                return;
            }

            // Destroy the projectile unless it hits the origin and should ignore it
            if (!(dontCollideWithOrigin && collision.gameObject == launcherOrigin))
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!(dontCollideWithOrigin && collision.gameObject == launcherOrigin))
            {
                Destroy(gameObject);
            }
        }

        private void OnValidate()
        {
            // Force lifetime to be 0 or greater
            if (lifetime < 0)
            {
                lifetime = 0;
            }
        }
    }
}
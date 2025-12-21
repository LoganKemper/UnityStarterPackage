// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Attach to a projectile prefab to give it launching and destroying behavior.
    /// </summary>
    public class Projectile3D : MonoBehaviour
    {
        [Tooltip("Drag in the projectile's Rigidbody.")]
        [SerializeField] private Rigidbody _rigidbody;

        [Tooltip("How long in seconds until the projectile disappears.")]
        [SerializeField] private float lifetime = 5f;

        [Tooltip("Multiplies the projectile's velocity set by the launch origin.")]
        [SerializeField] private float velocityMultiplier = 1f;

        private void Start()
        {
            // Destroy the projectile after its lifetime expires
            Destroy(gameObject, lifetime);
        }

        public void Launch(Transform launchTransform, float velocity)
        {
            _rigidbody.linearVelocity = velocity * velocityMultiplier * launchTransform.forward;
        }

        public void Launch(Vector3 direction, float velocity)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            _rigidbody.linearVelocity = velocity * velocityMultiplier * direction;
        }

        private void OnValidate()
        {
            // Force lifetime to be 0 or greater
            lifetime = Mathf.Max(0, lifetime);
        }
    }
}
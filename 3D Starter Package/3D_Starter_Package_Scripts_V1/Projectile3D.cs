// Unity Starter Package - Version 1
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
        [SerializeField] private Rigidbody m_Rigidbody;

        [Tooltip("How long in seconds until the projectile disappears.")]
        [SerializeField] private float lifetime = 5f;

        [Tooltip("Multiplies the projectile's velocity set by the launch origin.")]
        [SerializeField] private float velocityMultiplier = 1f;

        private void Start()
        {
            // Destroy the projectile after its lifetime expires
            Destroy(gameObject, lifetime);

            // If the Rigidbody hasn't been assigned, try to find it on this GameObject
            if (m_Rigidbody == null)
            {
                m_Rigidbody = GetComponent<Rigidbody>();
            }
        }

        public void Launch(Transform launchTransform, float velocity)
        {
            m_Rigidbody.linearVelocity = velocity * velocityMultiplier * launchTransform.forward;
        }

        public void Launch(Vector3 direction, float velocity)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            m_Rigidbody.linearVelocity = velocity * velocityMultiplier * direction;
        }

        private void OnValidate()
        {
            // Force lifetime to be 0 or greater
            lifetime = Mathf.Max(0, lifetime);
        }
    }
}
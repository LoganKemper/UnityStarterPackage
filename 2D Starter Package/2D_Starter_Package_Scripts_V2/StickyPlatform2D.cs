// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Add to a moving platform to make GameObjects stick to its surface.
    /// </summary>
    public class StickyPlatform2D : MonoBehaviour
    {
        private Transform target = null;
        private Vector3 offset;

        private void OnTriggerStay2D(Collider2D collision)
        {
            target = collision.gameObject.transform;
            offset = target.position - transform.position;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            target = null;
        }

        // LateUpdate() is called after Update()
        // This is done so that the platform moves the target after any other action is taken by the target
        private void LateUpdate()
        {
            if (target != null)
            {
                // Move the target with this GameObject
                target.position = transform.position + offset;
            }
        }
    }
}
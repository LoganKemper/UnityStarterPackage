// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Generic script for adding UnityEvents to 2D collisions.
    /// </summary>
    public class CollisionEvents2D : MonoBehaviour
    {
        [Tooltip("Enter the tag name that should register collisions. Leave blank for any tag to be used.")]
        [SerializeField] private string tagName = "Player";

        [Space(20)]
        [SerializeField] private UnityEvent onCollision, onCollisionExit;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Invokes onCollision if there's a collision on this GameObject and...
            // A) the tag field is empty
            // OR
            // B) the colliding GameObject's tag matches tagName

            if (string.IsNullOrEmpty(tagName) || collision.collider.CompareTag(tagName))
            {
                Collision();
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            // Invokes onCollisionExit if there's a collision exit on this GameObject and...
            // A) the tag field is empty
            // OR
            // B) the colliding GameObject's tag matches tagName

            if (string.IsNullOrEmpty(tagName) || collision.collider.CompareTag(tagName))
            {
                CollisionExit();
            }
        }

        [ContextMenu("Invoke Collision Event")]
        public void Collision()
        {
            onCollision.Invoke();
        }

        [ContextMenu("Invoke Collision Exit Event")]
        public void CollisionExit()
        {
            onCollisionExit.Invoke();
        }
    }
}
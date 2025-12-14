// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Adds a stealth mechanic that allows the player to be detected by enemies or be "in cover" to avoid detection.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PlayerStealth2D : MonoBehaviour
    {
        [Header("Cover Settings")]
        [Tooltip("Enter the name of the tag that should cause the player to enter cover.")]
        [SerializeField] private string coverTag = "Cover";

        [Space(20)]
        [SerializeField] private UnityEvent onEnteredCover;
        [Space(20)]
        [SerializeField] private UnityEvent onExitedCover;

        [Header("Detection Settings")]
        [Tooltip("Enter the name of the tag that should cause the player to be spotted.")]
        [SerializeField] private string detectionTag = "Detection";

        [Tooltip("Optional: Set a stealth respawn point to make the player respawn there if they are detected.")]
        [SerializeField] private Transform stealthRespawn;

        [Space(20)]
        [SerializeField] private UnityEvent onDetected;

        // Is the player currently in cover?
        public bool InCover => coverCount > 0;

        // Tracks how many cover colliders the player is currently inside
        private int coverCount = 0;

        // Call from a UnityEvent to set the stealth respawn manually
        public void SetStealthRespawn(Transform newRespawn)
        {
            stealthRespawn.SetPositionAndRotation(newRespawn.position, newRespawn.rotation);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsCover(collision))
            {
                if (coverCount == 0)
                {
                    onEnteredCover.Invoke();
                }

                coverCount++;
            }

            if (IsDetected(collision))
            {
                if (stealthRespawn != null)
                {
                    transform.position = stealthRespawn.position;
                }

                onDetected.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (IsCover(collision))
            {
                coverCount = Mathf.Max(0, coverCount - 1);

                if (coverCount == 0)
                {
                    onExitedCover.Invoke();
                }
            }
        }

        private bool IsCover(Collider2D collider)
        {
            return collider.CompareTag(coverTag);
        }

        private bool IsDetected(Collider2D collider)
        {
            return collider.CompareTag(detectionTag) && !InCover;
        }
    }
}
// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Teleports the player to another location.
    /// </summary>
    public class Teleporter3D : MonoBehaviour
    {
        [Tooltip("Enter the player's tag name. Could be used for other tags as well.")]
        [SerializeField] private string tagName = "Player";

        [Tooltip("Drag in the transform that the player will be teleported to.")]
        [SerializeField] private Transform destination;

        [Tooltip("If true, the player will be rotated to match the rotation of the destination transform.")]
        [SerializeField] private bool useDestinationRotation = false;

        [Tooltip("If true, the teleport key must be pressed once the player has entered the trigger. If false, they will be teleported as soon as they enter the trigger.")]
        [SerializeField] private bool requireKeyPress = true;

        [Tooltip("Optional: Cooldown after teleport to prevent immediate re-teleports.")]
        [SerializeField] private float teleportCooldown = 0.5f;

        [Tooltip("The key input that the script is listening for.")]
        [SerializeField] private KeyCode teleportKey = KeyCode.Space;

        [Space(20)]
        [SerializeField] private UnityEvent onTeleported;

        private Transform playerRoot;
        private float nextAllowedTeleportTime;
        private int playerColliderCount;

        private void Update()
        {
            if (Input.GetKeyDown(teleportKey) && requireKeyPress && playerRoot != null)
            {
                TryTeleport();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!string.IsNullOrEmpty(tagName) && other.CompareTag(tagName))
            {
                // Try to get the root of the player from its Rigidbody
                Transform root = other.attachedRigidbody != null ? other.attachedRigidbody.transform : other.transform;

                if (playerRoot == null)
                {
                    playerRoot = root;
                }

                playerColliderCount++;

                if (!requireKeyPress)
                {
                    TryTeleport();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!string.IsNullOrEmpty(tagName) && other.CompareTag(tagName))
            {
                // Only decrement if this exit belongs to the same player object
                Transform root = other.attachedRigidbody != null ? other.attachedRigidbody.transform : other.transform;

                if (root != playerRoot)
                {
                    return;
                }

                playerColliderCount = Mathf.Max(0, playerColliderCount - 1);

                if (playerColliderCount == 0)
                {
                    playerRoot = null;
                }
            }
        }

        private void TryTeleport()
        {
            if (playerRoot == null)
            {
                return;
            }

            if (Time.time < nextAllowedTeleportTime)
            {
                return;
            }

            TeleportPlayer();
            nextAllowedTeleportTime = Time.time + teleportCooldown;
        }

        // Teleport the player to the specified destination
        public void TeleportPlayer()
        {
            if (destination == null)
            {
                Debug.LogWarning("Teleporter destination is not assigned");
                return;
            }

            if (playerRoot == null)
            {
                Debug.LogWarning("No valid player to teleport");
                return;
            }

            if (useDestinationRotation)
            {
                // Move and rotate the player to the destination transform
                playerRoot.SetPositionAndRotation(destination.position, destination.rotation);
            }
            else
            {
                // Only move the player to the destination transform
                playerRoot.position = destination.position;
            }

            onTeleported.Invoke();
        }
    }
}
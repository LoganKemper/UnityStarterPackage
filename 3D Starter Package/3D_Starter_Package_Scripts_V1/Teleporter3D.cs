// Unity Starter Package - Version 1
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

        [Tooltip("The key input that the script is listening for.")]
        [SerializeField] private KeyCode teleportKey = KeyCode.Space;

        [Space(20)]
        [SerializeField] private UnityEvent onTeleported;

        private Transform player;

        private void Update()
        {
            if (Input.GetKeyDown(teleportKey) && requireKeyPress && player != null)
            {
                TeleportPlayer();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!string.IsNullOrEmpty(tagName) && other.CompareTag(tagName))
            {
                player = other.transform;

                if (!requireKeyPress)
                {
                    TeleportPlayer();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!string.IsNullOrEmpty(tagName) && other.CompareTag(tagName))
            {
                player = null;
            }
        }

        // Teleport the player to the specified destination
        public void TeleportPlayer()
        {
            if (destination == null)
            {
                Debug.LogWarning("Teleporter destination is not assigned");
                return;
            }

            if (useDestinationRotation)
            {
                // Move and rotate the player to the destination transform
                player.SetPositionAndRotation(destination.position, destination.rotation);
            }
            else
            {
                // Only move the player to the destination transform
                player.position = destination.position;
            }

            onTeleported.Invoke();
        }
    }
}
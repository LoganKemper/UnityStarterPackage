// Unity Starter Package - Version 3
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Teleports the player to another location.
    /// </summary>
    public class Teleporter2D : MonoBehaviour
    {
        [Tooltip("Enter the player's tag name. Could be used for other tags as well.")]
        [SerializeField] private string tagName = "Player";

        [Tooltip("Drag in the transform that the player will be teleported to.")]
        [SerializeField] private Transform destination;

        [Tooltip("If true, the teleport key must be pressed once the player has entered the trigger. If false, they will be teleported as soon as they enter the trigger.")]
        [SerializeField] private bool requireKeyPress = true;

        [Tooltip("The input action used for teleporting. Set to the space bar by default.")]
        [SerializeField] private InputAction teleportAction = new("Teleport", InputActionType.Button, "<Keyboard>/space");

        [Space(20)]
        [SerializeField] private UnityEvent onTeleported;

        private Transform player;

        private void OnEnable()
        {
            teleportAction.Enable();
        }

        private void OnDisable()
        {
            teleportAction.Disable();
        }

        private void Update()
        {
            if (teleportAction.WasPressedThisFrame() && requireKeyPress && player != null)
            {
                TeleportPlayer();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!string.IsNullOrEmpty(tagName) && collision.CompareTag(tagName))
            {
                player = collision.transform;

                if (!requireKeyPress)
                {
                    TeleportPlayer();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!string.IsNullOrEmpty(tagName) && collision.CompareTag(tagName))
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

            player.position = destination.position;
            onTeleported.Invoke();
        }
    }
}

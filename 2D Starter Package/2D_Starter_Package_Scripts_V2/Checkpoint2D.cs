// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Used to update the player's respawn position. Attach this to a GameObject with a 2D collider set to trigger. 
    /// Alternatively, call SetRespawnPoint from a UnityEvent.
    /// </summary>
    public class Checkpoint2D : MonoBehaviour
    {
        [Tooltip("Assign the respawn point GameObject.")]
        [SerializeField] private Transform respawn;

        [Tooltip("Enter the tag name that should register triggers. Leave blank for any tag to be used.")]
        [SerializeField] private string tagName = "Player";

        [Tooltip("If true, this checkpoint can only be activated once.")]
        [SerializeField] private bool singleUse = false;

        [Space(20)]
        [SerializeField] private UnityEvent onCheckpoint;

        private bool used;

        private void Start()
        {
            // If the respawn transform has not been assigned, try to find it by tag
            if (respawn == null)
            {
                GameObject respawnObject = GameObject.FindGameObjectWithTag("Respawn");
                if (respawnObject != null)
                {
                    respawn = respawnObject.transform;
                }
                else
                {
                    Debug.LogWarning("Respawn point is not assigned");
                }
            }
        }

        // If called from a UnityEvent, make sure to pass in the transform of the new respawn point
        public void SetRespawnPoint(Transform respawnPoint)
        {
            if (respawn != null && respawnPoint != null)
            {
                respawn.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
                onCheckpoint.Invoke();
            }
            else
            {
                Debug.LogWarning("Cannot set respawn point because the transform is null");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (singleUse)
            {
                if (!used)
                {
                    if (string.IsNullOrEmpty(tagName) || collision.CompareTag(tagName))
                    {
                        SetRespawnPoint(transform);
                        used = true;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(tagName) || collision.CompareTag(tagName))
                {
                    SetRespawnPoint(transform);
                }
            }
        }
    }
}
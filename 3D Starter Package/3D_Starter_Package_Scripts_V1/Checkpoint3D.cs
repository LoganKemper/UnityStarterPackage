// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Used to update the player's respawn position. Attach this to a GameObject with a 3D collider set to trigger. 
    /// Alternatively, call SetRespawnPoint from a UnityEvent.
    /// </summary>
    public class Checkpoint3D : MonoBehaviour
    {
        [Tooltip("Assign the respawn point GameObject.")]
        [SerializeField] private Transform respawn;

        [Tooltip("Enter the tag name that should register triggers. Leave blank for any tag to be used.")]
        [SerializeField] private string tagName = "Player";

        [Tooltip("If true, this checkpoint can only be activated once.")]
        [SerializeField] private bool singleUse = false;

        [Space(20)]
        [SerializeField] private UnityEvent onCheckpoint;

        private bool used = false;

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

        private void OnTriggerEnter(Collider other)
        {
            if (singleUse)
            {
                if (!used)
                {
                    if (string.IsNullOrEmpty(tagName) || other.CompareTag(tagName))
                    {
                        SetRespawnPoint(transform);
                        used = true;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(tagName) || other.CompareTag(tagName))
                {
                    SetRespawnPoint(transform);
                }
            }
        }
    }
}
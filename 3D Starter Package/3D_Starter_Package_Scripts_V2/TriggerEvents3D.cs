// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Generic script for adding UnityEvents to 2D trigger collisions.
    /// </summary>
    public class TriggerEvents3D : MonoBehaviour
    {
        [Tooltip("Enter the tag name that should register triggers. Leave blank for any tag to be used.")]
        [SerializeField] private string tagName = "Player";

        [Space(20)]
        [SerializeField] private UnityEvent onTrigger, onTriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            // Invokes onTriggerEvent if there's a trigger enter on this GameObject and...
            // A) the tag field is empty
            // OR
            // B) the triggering GameObject's tag matches tagName

            if (string.IsNullOrEmpty(tagName) || other.CompareTag(tagName))
            {
                Trigger();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Invokes onExitEvent if there's a trigger exit on this GameObject and...
            // A) the tag field is empty
            // OR
            // B) the triggering GameObject's tag matches tagName

            if (string.IsNullOrEmpty(tagName) || other.CompareTag(tagName))
            {
                TriggerExit();
            }
        }

        [ContextMenu("Invoke Trigger Event")]
        public void Trigger()
        {
            onTrigger.Invoke();
        }

        [ContextMenu("Invoke Trigger Exit Event")]
        public void TriggerExit()
        {
            onTriggerExit.Invoke();
        }
    }
}
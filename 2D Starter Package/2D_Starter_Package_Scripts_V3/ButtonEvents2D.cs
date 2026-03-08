// Unity Starter Package - Version 3
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Generic script for adding UnityEvents to button presses.
    /// </summary>
    public class ButtonEvents2D : MonoBehaviour
    {
        [Tooltip("Enter the tag name that should register triggers. Leave blank for any tag to be used.")]
        [SerializeField] private string tagName = "Player";

        [Tooltip("If false, the button can be activated at any time.")]
        [SerializeField] private bool requiresTrigger = true;

        [Tooltip("The input action that the script is listening for. Set to the E key by default. Remove all bindings to disable input.")]
        [SerializeField] private InputAction buttonAction = new("Activate", InputActionType.Button, "<Keyboard>/e");

        [Space(20)]
        [SerializeField] private UnityEvent onButtonActivated, onButtonReleased;

        private bool entered;

        private void OnEnable()
        {
            buttonAction.Enable();
        }

        private void OnDisable()
        {
            buttonAction.Disable();
        }

        private void Update()
        {
            // Prevents unnecessary checks if no bindings have been assigned
            if (buttonAction.bindings.Count == 0)
            {
                return;
            }

            if (buttonAction.WasPressedThisFrame() && (entered || !requiresTrigger))
            {
                ButtonActivated();
            }
            else if (buttonAction.WasReleasedThisFrame() && (entered || !requiresTrigger))
            {
                ButtonReleased();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (string.IsNullOrEmpty(tagName) || collision.CompareTag(tagName))
            {
                entered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (string.IsNullOrEmpty(tagName) || collision.CompareTag(tagName))
            {
                entered = false;
            }
        }

        [ContextMenu("Invoke Button Activated Event")]
        public void ButtonActivated()
        {
            onButtonActivated.Invoke();
        }

        [ContextMenu("Invoke Button Released Event")]
        public void ButtonReleased()
        {
            onButtonReleased.Invoke();
        }
    }
}

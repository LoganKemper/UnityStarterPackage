// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic script for adding UnityEvents to button presses.
/// </summary>
public class ButtonEvents2D : MonoBehaviour
{
    [Tooltip("Enter the tag name that should register triggers. Leave blank for any tag to be used.")]
    [SerializeField] private string tagName;

    [Tooltip("If false, the button can be activated at any time.")]
    [SerializeField] private bool requiresTrigger = true;

    [Tooltip("The key input that the script is listening for. Choose \"None\" to disable key input.")]
    [SerializeField] private KeyCode keyToPress = KeyCode.E;

    [Space(20)]
    [SerializeField] private UnityEvent onButtonActivated, onButtonReleased;

    private bool entered;

    private void Update()
    {
        // Prevents unnecessary checks if no key has been assigned
        if (keyToPress == KeyCode.None)
        {
            return;
        }

        if (Input.GetKeyDown(keyToPress) && (entered || !requiresTrigger))
        {
            onButtonActivated.Invoke();
        }
        else if (Input.GetKeyUp(keyToPress) && (entered || !requiresTrigger))
        {
            onButtonReleased.Invoke();
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
}

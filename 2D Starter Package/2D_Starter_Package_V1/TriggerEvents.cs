using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic script for adding UnityEvents to 2D trigger collisions.
/// </summary>
public class TriggerEvents : MonoBehaviour
{
    [Tooltip("Enter the tag name that should register triggers. Leave blank for any tag to be used.")]
    [SerializeField] private string tagName = "Player";

    [Space(20)]
    [SerializeField] private UnityEvent onTrigger, onTriggerExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Invokes onTriggerEvent if there's a trigger enter on this GameObject and...
        // A) the tag field is empty
        // OR
        // B) the triggering GameObject's tag matches tagName

        if (string.IsNullOrEmpty(tagName) || collision.CompareTag(tagName))
        {
            onTrigger.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Invokes onExitEvent if there's a trigger exit on this GameObject and...
        // A) the tag field is empty
        // OR
        // B) the triggering GameObject's tag matches tagName

        if (string.IsNullOrEmpty(tagName) || collision.CompareTag(tagName))
        {
            onTriggerExit.Invoke();
        }
    }
}

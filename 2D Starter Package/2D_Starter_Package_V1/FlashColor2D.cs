// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;

/// <summary>
/// Changes a SpriteRenderer to a new color, then back to the original color.
/// </summary>
public class FlashColor2D : MonoBehaviour
{
    [Tooltip("Drag in the SpriteRenderer component.")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Color that the sprite will flash to.")]
    [SerializeField] private Color flashColor = Color.red;

    [Tooltip("Length of the flash in seconds.")]
    [SerializeField] private float flashDuration = 0.1f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Start()
    {
        // Cache the original color
        originalColor = spriteRenderer.color;
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    // Flash to a color, then change back to the original color
    private IEnumerator FlashCoroutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    // Reset if this GameObject is disabled
    private void OnDisable()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            spriteRenderer.color = originalColor;
        }
    }

    private void OnValidate()
    {
        // Prevent flashDuration from being set to negative
        if (flashDuration < 0)
        {
            flashDuration = 0;
        }
    }
}

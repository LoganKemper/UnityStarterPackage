// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Changes a MeshRenderer's material color to a new color, then back to the original color.
    /// </summary>
    public class FlashColor3D : MonoBehaviour
    {
        [Tooltip("Drag in the MeshRenderer component.")]
        [SerializeField] private MeshRenderer meshRenderer;

        [Tooltip("Color that the material will flash to.")]
        [SerializeField] private Color flashColor = Color.red;

        [Tooltip("Length of the flash in seconds.")]
        [SerializeField] private float flashDuration = 0.1f;

        private Material materialInstance;
        private Coroutine flashCoroutine;
        private Color originalColor;

        private void Start()
        {
            // Cache the original material color by instantiating a unique material instance
            if (meshRenderer != null)
            {
                materialInstance = meshRenderer.material; // This creates a copy of the material
                originalColor = materialInstance.color;
            }
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
            materialInstance.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            materialInstance.color = originalColor;
        }

        // Reset if this GameObject is disabled
        private void OnDisable()
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                if (materialInstance != null)
                {
                    materialInstance.color = originalColor;
                }
            }
        }

        private void OnValidate()
        {
            // Prevent flashDuration from being set to negative
            flashDuration = Mathf.Max(flashDuration, 0);
        }
    }
}
// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Add to a player controller to interact with nearby objects. Intended for first-person controllers, but can work with other perspectives.
    /// </summary>
    public class RaycastInteractor : MonoBehaviour
    {
        [Tooltip("Drag in the transform the interactor should come from. Likely the camera on a first-person controller.")]
        [SerializeField] private Transform interactorSource;

        [Tooltip("The maximum allowed distance from the camera to the interactable GameObject.")]
        [SerializeField] private float interactDistance = 5f;

        [Tooltip("Choose a interaction key input.")]
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        private void Update()
        {
            if (Input.GetKeyDown(interactKey))
            {
                Interact();
            }
        }

        private void Interact()
        {
            // Sends a ray out from the camera
            Ray ray = new(interactorSource.position, interactorSource.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                // If the ray hits a GameObject with RaycastInteractable attached, call Interaction() on it
                RaycastInteractable interactable = hit.collider.GetComponent<RaycastInteractable>();
                if (interactable != null)
                {
                    interactable.Interaction();
                }
            }

            // Draws the ray in the scene view for one second
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green, 1f);
        }
    }
}
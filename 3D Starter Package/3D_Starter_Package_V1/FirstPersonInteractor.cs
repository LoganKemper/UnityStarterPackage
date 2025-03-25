using UnityEngine;

/// <summary>
/// Add to a first-person controller to interact with nearby objects.
/// </summary>
public class FirstPersonInteractor : MonoBehaviour
{
    [Tooltip("Drag in the first-person camera.")]
    [SerializeField] private Camera playerCamera;

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
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);

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

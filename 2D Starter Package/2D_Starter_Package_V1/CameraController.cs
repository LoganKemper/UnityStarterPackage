using UnityEngine;

/// <summary>
/// Attach to the main camera to control its movement.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Tooltip("The transform that the camera will follow.")]
    [SerializeField] private Transform target;

    [Tooltip("How quickly the camera keeps up with the target. A higher value will follow the target more closely.")]
    [SerializeField] private float followSpeed = 10f;

    [Tooltip("Allows the player to peek with up/down input.")]
    [SerializeField] private bool allowPeeking = true;

    [Tooltip("Maximum offset allowed when peeking up or down.")]
    [SerializeField] private float maxPeekDistance = 2f;

    [Tooltip("Camera's position offset from the target.")]
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Tooltip("Minimum (x, y) bounds for the camera.")]
    [SerializeField] private Vector2 minBounds = new(-500f, -500f);

    [Tooltip("Maximum (x, y) bounds for the camera.")]
    [SerializeField] private Vector2 maxBounds = new(500f, 500f);

    // Enable or disable peeking from a UnityEvent with this method
    public void EnablePeeking(bool peekingEnabled)
    {
        allowPeeking = peekingEnabled;
    }

    // LateUpdate() is called after Update()
    // This is done so that the camera isn't moved until the positions of everything are finalized
    private void LateUpdate()
    {
        // Return early if the target has not been assigned
        if (target == null)
        {
            return;
        }

        // Calculate the desired position of the camera, factoring in the offsets
        Vector2 desiredPosition = (Vector2)target.position + offset;

        // Add a y-offset based on the player's moveY input to allow peeking
        float peekOffset = 0.0f;

        // If peeking is enabled, get the up/down input
        if (allowPeeking)
        {
            peekOffset = Input.GetAxisRaw("Vertical");
        }

        float peekOffsetY = Mathf.Clamp(peekOffset * maxPeekDistance, -maxPeekDistance, maxPeekDistance);
        desiredPosition.y += peekOffsetY;

        // Clamp the camera's position within the bounds
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

        // Create a target position with the clamped values
        Vector3 clampedPosition = new(clampedX, clampedY, transform.position.z);

        // Interpolate between the current position and the target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, 1 - Mathf.Exp(-followSpeed * Time.deltaTime));

        // Update the camera's position
        transform.position = smoothedPosition;
    }

    // Draws a box in the scene view visualizing the camera bounds
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(minBounds.x, minBounds.y), new Vector2(minBounds.x, maxBounds.y));
        Gizmos.DrawLine(new Vector2(minBounds.x, maxBounds.y), new Vector2(maxBounds.x, maxBounds.y));
        Gizmos.DrawLine(new Vector2(maxBounds.x, maxBounds.y), new Vector2(maxBounds.x, minBounds.y));
        Gizmos.DrawLine(new Vector2(maxBounds.x, minBounds.y), new Vector2(minBounds.x, minBounds.y));
    }

    private void OnValidate()
    {
        // Make sure followSpeed can't be set negative in the inspector
        if (followSpeed < 0)
        {
            followSpeed = 0;
        }

        // Make sure maxPeekDistance can't be set negative in the inspector
        if (maxPeekDistance < 0)
        {
            maxPeekDistance = 0;
        }
    }
}

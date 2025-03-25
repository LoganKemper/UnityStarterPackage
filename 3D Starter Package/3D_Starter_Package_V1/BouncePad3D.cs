using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A bounce pad that applies an instant force to any GameObject with a Rigidbody that collides with it.
/// </summary>
public class BouncePad3D : MonoBehaviour
{
    [Tooltip("Enter the tag name that should register collisions. Leave blank for any object to be affected.")]
    [SerializeField] private string tagName;

    [Tooltip("The amount of force applied to the colliding GameObject's Rigidbody2D.")]
    [SerializeField] private float bounceForce = 25f;

    [Tooltip("Choose the direction of the bounce force relative to the bounce pad.")]
    [SerializeField] private BounceDirection bounceDirection = BounceDirection.Up;

    [Space(20)]
    [SerializeField] private UnityEvent onBounce;

    // Defines possible bounce directions
    public enum BounceDirection
    {
        Up,
        Down,
        Forward,
        Backward,
        Left,
        Right,
        UpForward,
        UpBackward,
        UpLeft,
        UpRight,
        DownForward,
        DownBackward,
        DownLeft,
        DownRight
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object has the required tag (if specified)
        if (string.IsNullOrEmpty(tagName) || collision.collider.CompareTag(tagName))
        {
            // Try to access the rigidbody on the colliding GameObject
            if (collision.gameObject.TryGetComponent(out Rigidbody rb))
            {
                // Determine the local direction vector and convert it to world space
                Vector3 localDirection = GetDirectionVector(bounceDirection);
                Vector3 worldDirection = transform.TransformDirection(localDirection);

                // Apply impulse (instant) force to the rigidbody in world space
                rb.AddForce(worldDirection * bounceForce, ForceMode.Impulse);
                onBounce.Invoke();
            }
        }
    }

    // Convert the bounce direction into a unit vector
    private Vector3 GetDirectionVector(BounceDirection direction)
    {
        return direction switch
        {
            BounceDirection.Up => Vector3.up,
            BounceDirection.Down => Vector3.down,
            BounceDirection.Forward => Vector3.forward,
            BounceDirection.Backward => Vector3.back,
            BounceDirection.Left => Vector3.left,
            BounceDirection.Right => Vector3.right,
            BounceDirection.UpForward => new Vector3(0, 1, 1).normalized,
            BounceDirection.UpBackward => new Vector3(0, 1, -1).normalized,
            BounceDirection.UpLeft => new Vector3(-1, 1, 0).normalized,
            BounceDirection.UpRight => new Vector3(1, 1, 0).normalized,
            BounceDirection.DownForward => new Vector3(0, -1, 1).normalized,
            BounceDirection.DownBackward => new Vector3(0, -1, -1).normalized,
            BounceDirection.DownLeft => new Vector3(-1, -1, 0).normalized,
            BounceDirection.DownRight => new Vector3(1, -1, 0).normalized,
            _ => Vector3.up
        };
    }
}

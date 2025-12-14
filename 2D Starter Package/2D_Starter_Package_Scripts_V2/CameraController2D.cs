// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Attach to the main camera to control its movement.
    /// </summary>
    public class CameraController2D : MonoBehaviour
    {
        [System.Serializable]
        public class ViewDestinationSettings
        {
            [Tooltip("In seconds, how long the destination will be viewed for (including time in transit).")]
            public float viewTime = 2f;

            [Space(10)]
            public UnityEvent onViewBegan, onViewEnded;
        }

        [Tooltip("The transform that the camera will follow (likely the player).")]
        [SerializeField] private Transform target;

        [Tooltip("How quickly the camera keeps up with the target. A higher value will follow the target more closely.")]
        [SerializeField] private float followSpeed = 7.5f;

        [Tooltip("Allows the player to peek with up/down input.")]
        [SerializeField] private bool allowPeeking = false;

        [Tooltip("Maximum offset allowed when peeking up or down.")]
        [SerializeField] private float maxPeekDistance = 2f;

        [Tooltip("Camera's position offset from the target.")]
        [SerializeField] private Vector2 offset = Vector2.zero;

        [Tooltip("Minimum (x, y) bounds for the camera.")]
        [SerializeField] private Vector2 minBounds = new(-500f, -500f);

        [Tooltip("Maximum (x, y) bounds for the camera.")]
        [SerializeField] private Vector2 maxBounds = new(500f, 500f);

        [SerializeField] private ViewDestinationSettings viewDestinationSettings;

        private Coroutine cameraCoroutine;
        private Transform cachedTarget;
        private bool viewingDestination = false;
        private bool pauseFollowing = false;

        // Assign a new target transform
        public void SetTarget(Transform target)
        {
            if (target == null)
            {
                return;
            }

            if (viewingDestination)
            {
                cachedTarget = target;
            }
            else
            {
                this.target = target;
            }
        }

        // Set the position of the camera
        public void SetPosition(Transform targetPosition)
        {
            if (targetPosition == null)
            {
                return;
            }

            // Set the camera's position to the target position, keeping the current z-value
            Vector3 newPosition = targetPosition.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }

        // Set a new camera follow speed
        public void SetFollowSpeed(float followSpeed)
        {
            this.followSpeed = followSpeed;
        }

        // Enable or disable peeking
        public void SetPeekingEnabled(bool allowPeeking)
        {
            this.allowPeeking = allowPeeking;
        }

        // Set a new peek distance
        public void SetMaxPeekDistance(float maxPeekDistance)
        {
            this.maxPeekDistance = maxPeekDistance;
        }

        // Instantly snap to the target (useful for player teleportation)
        public void SnapToTarget()
        {
            if (target == null)
            {
                return;
            }

            // Set the camera's position to the target position, keeping the current z-value
            Vector3 newPosition = target.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }

        // Instantly snap to the target, factoring in the offset
        public void SnapToTargetWithOffset()
        {
            if (target == null)
            {
                return;
            }

            // Set the camera's position to the target position with the offset, keeping the current z-value
            Vector3 newPosition = target.position + (Vector3)offset;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }

        // Pause the camera following the target
        public void PauseFollowing(bool pauseFollowing)
        {
            this.pauseFollowing = pauseFollowing;
        }

        // Toggle the camera following
        [ContextMenu("Toggle Following")]
        public void ToggleFollowing()
        {
            pauseFollowing = !pauseFollowing;
        }

        // Hold the camera at its current position
        public void HoldPosition(float holdSeconds)
        {
            if (viewingDestination)
            {
                return;
            }

            if (cameraCoroutine != null)
            {
                StopCoroutine(cameraCoroutine);
            }

            cameraCoroutine = StartCoroutine(HoldPositionCoroutine(holdSeconds));
        }

        // Send the camera to look at a destination transform
        public void ViewDestination(Transform destination)
        {
            if (viewingDestination || destination == null)
            {
                return;
            }

            if (cameraCoroutine != null)
            {
                StopCoroutine(cameraCoroutine);
            }

            cameraCoroutine = StartCoroutine(ViewDestinationCoroutine(destination));
        }

        // Set the number of seconds the camera will look at the destination
        public void SetViewTime(float seconds)
        {
            viewDestinationSettings.viewTime = Mathf.Max(0f, seconds);
        }

        // LateUpdate() is called after Update()
        // This is done so that the camera isn't moved until the positions of everything else are finalized
        private void LateUpdate()
        {
            // Return early if the target has not been assigned or following is paused
            if (target == null || pauseFollowing)
            {
                return;
            }

            // Calculate the desired position of the camera, factoring in the offsets
            Vector2 desiredPosition = (Vector2)target.position;

            // Add a Y-offset based on the player's moveY input to allow peeking
            float peekOffset = 0f;

            // If peeking is enabled, get the up/down input
            if (allowPeeking)
            {
                peekOffset = Input.GetAxisRaw("Vertical");
            }

            // Add offsets if not in viewing destination mode
            if (!viewingDestination)
            {
                desiredPosition += offset;

                float peekOffsetY = Mathf.Clamp(peekOffset * maxPeekDistance, -maxPeekDistance, maxPeekDistance);
                desiredPosition.y += peekOffsetY;
            }

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

        private IEnumerator HoldPositionCoroutine(float holdSeconds)
        {
            pauseFollowing = true;
            yield return new WaitForSeconds(holdSeconds);
            pauseFollowing = false;
            cameraCoroutine = null;
        }

        private IEnumerator ViewDestinationCoroutine(Transform destination)
        {
            viewingDestination = true;
            cachedTarget = target;
            target = destination;
            viewDestinationSettings.onViewBegan.Invoke();
            yield return new WaitForSeconds(viewDestinationSettings.viewTime);
            target = cachedTarget;
            viewingDestination = false;
            viewDestinationSettings.onViewEnded.Invoke();
            cameraCoroutine = null;
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
            // Clamp non-negative fields
            followSpeed = Mathf.Max(0, followSpeed);
            maxPeekDistance = Mathf.Max(0, maxPeekDistance);
            viewDestinationSettings.viewTime = Mathf.Max(0f, viewDestinationSettings.viewTime);
        }
    }
}
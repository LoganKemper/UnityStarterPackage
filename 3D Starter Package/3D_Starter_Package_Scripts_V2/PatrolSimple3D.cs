// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Moves a GameObject back and forth between two positions. Can be used for platforms, NPCs, hazards, and more.
    /// </summary>
    public class PatrolSimple3D : MonoBehaviour
    {
        private enum MovementType : byte
        {
            Translation,
            Rigidbody
        }

        [Tooltip("The other position that this GameObject should move to.")]
        [SerializeField] private Transform pointB;

        [Tooltip("How fast the patrolling object should move.")]
        [SerializeField] private float speed = 3f;

        [Tooltip("How close this GameObject needs to be from the end points to switch target. May need to be adjusted depending on the scale of your game.")]
        [SerializeField] private float distanceThreshold = 0.05f;

        [Tooltip("Choose whether to translate the position of this object, or affect its rigidbody. Rigidbody movement will be the better option for physics collisions.")]
        [SerializeField] private MovementType movementType = MovementType.Translation;

        private Rigidbody rb;
        private Vector3 pointAPosition;
        private bool toB = true;

        private void Start()
        {
            // If using rigidbody movement, get the rigidbody component and set it up
            if (movementType == MovementType.Rigidbody)
            {
                rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }

            // Save the initial position as point A
            pointAPosition = transform.position;
        }

        // Perform translation in Update
        private void Update()
        {
            if (movementType != MovementType.Translation)
            {
                return;
            }

            if (toB)
            {
                transform.position = Vector3.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, pointB.position) < distanceThreshold)
                {
                    toB = false;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, pointAPosition, speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, pointAPosition) < distanceThreshold)
                {
                    toB = true;
                }
            }
        }

        // Perform rigidbody movement in FixedUpdate
        private void FixedUpdate()
        {
            if (movementType != MovementType.Rigidbody || rb == null)
            {
                return;
            }

            if (toB)
            {
                Vector3 next = Vector3.MoveTowards(rb.position, pointB.position, speed * Time.fixedDeltaTime);
                rb.MovePosition(next);

                if (Vector3.Distance(rb.position, pointB.position) < distanceThreshold)
                {
                    toB = false;
                }
            }
            else
            {
                Vector3 next = Vector3.MoveTowards(rb.position, pointAPosition, speed * Time.fixedDeltaTime);
                rb.MovePosition(next);

                if (Vector3.Distance(rb.position, pointAPosition) < distanceThreshold)
                {
                    toB = true;
                }
            }
        }

        // Draws a line in the scene view to visualize the patrol path
        private void OnDrawGizmosSelected()
        {
            if (pointB == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, pointB.position);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }
    }
}
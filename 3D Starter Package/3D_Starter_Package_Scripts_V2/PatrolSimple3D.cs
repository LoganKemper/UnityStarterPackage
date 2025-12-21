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
        [Tooltip("The other position that this GameObject should move to.")]
        [SerializeField] private Transform pointB;

        [Tooltip("How fast the patrolling object should move.")]
        [SerializeField] private float speed = 3f;

        [Tooltip("How close this GameObject needs to be from the end points to switch target. May need to be adjusted depending on the scale of your game.")]
        [SerializeField] private float distanceThreshold = 0.05f;

        private Vector3 pointAPosition;
        private bool isRight = true;

        private void Start()
        {
            pointAPosition = transform.position;
        }

        private void Update()
        {
            if (isRight)
            {
                transform.position = Vector3.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, pointB.position) < distanceThreshold)
                {
                    isRight = false;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, pointAPosition, speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, pointAPosition) < distanceThreshold)
                {
                    isRight = true;
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
// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Moves a GameObject back and forth between two positions. Can be used for platforms, NPCs, hazards, and more.
    /// </summary>
    /// 
    public class PatrolSimple2D : MonoBehaviour
    {
        [Tooltip("The other position that this GameObject should move to.")]
        [SerializeField] private Transform pointB;

        [Tooltip("How fast the patrolling object should move.")]
        [SerializeField] private float speed = 3f;

        [Tooltip("How close this GameObject needs to be from the end points to switch target. May need to be adjusted depending on the scale of your game.")]
        [SerializeField] private float distanceThreshold = 0.05f;

        [Tooltip("Set this to true if you want this GameObject to flip itself on the x-axis when it reaches the end of its patrol path.")]
        [SerializeField] private bool flipOnDirectionChanged;

        private bool isRight = true;
        private Vector3 pointAPosition;

        private void Start()
        {
            pointAPosition = new Vector3(transform.position.x, transform.position.y, 0);
        }

        private void Update()
        {
            Vector3 thisPosition = new(transform.position.x, transform.position.y, 0);

            if (isRight)
            {
                transform.position = Vector3.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);

                if (Vector2.Distance(thisPosition, pointB.position) < distanceThreshold)
                {
                    isRight = false;

                    if (flipOnDirectionChanged)
                    {
                        FlipByScale();
                    }
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, pointAPosition, speed * Time.deltaTime);

                if (Vector2.Distance(thisPosition, pointAPosition) < distanceThreshold)
                {
                    isRight = true;

                    if (flipOnDirectionChanged)
                    {
                        FlipByScale();
                    }
                }
            }
        }

        public void FlipByScale()
        {
            // Flip sprite by multiplying x by -1
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        // Draws a line in the scene view to visualize the patrol path
        private void OnDrawGizmosSelected()
        {
            if (pointB == null)
            {
                return;
            }

            if (Camera.current == null)
            {
                return;
            }

            // Fixed gizmo size at any scale
            Vector3 pos = transform.position;
            Vector3 screenPos = Camera.current.WorldToScreenPoint(pos);
            Vector3 screenPos2 = screenPos + Vector3.right * 10f;
            Vector3 worldPos2 = Camera.current.ScreenToWorldPoint(screenPos2);
            float worldSize = (worldPos2 - pos).magnitude;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, pointB.position);
            Gizmos.DrawSphere(pointB.position, worldSize);
        }
    }
}